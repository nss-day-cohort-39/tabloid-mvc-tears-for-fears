using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public class PostTagRepository : BaseRepository
    {
        public PostTagRepository(IConfiguration config) : base(config) { }

        public List<Tag> GetAllRelationshipTagsByPostId(int postId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            pt.Id,
                            pt.PostId,
                            pt.TagId,
                            t.[Name]
                        FROM PostTag pt
                        LEFT JOIN Tag t ON t.Id = pt.TagId
                        WHERE pt.PostId = @postId";

                    cmd.Parameters.AddWithValue("@postId", postId);
                    var reader = cmd.ExecuteReader();

                    var tags = new List<Tag>();

                    while (reader.Read())
                    {
                        tags.Add(new Tag()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TagId")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                        });
                    }

                    reader.Close();

                    return tags;
                }
            }
        }

        public List<Tag> GetTagsWORelationshipByPostId(int postId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT * 
                    FROM Tag t
                    LEFT OUTER JOIN PostTag pt
                    ON(t.id = pt.TagId AND pt.PostId = @postId)
                    WHERE pt.TagId IS NULL";

                    cmd.Parameters.AddWithValue("@postId", postId);
                    var reader = cmd.ExecuteReader();

                    var tags = new List<Tag>();

                    while (reader.Read())
                    {
                        tags.Add(new Tag()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                        });
                    }

                    reader.Close();

                    return tags;
                }
            }
        }

        private Tag NewTagFromReader(SqlDataReader reader)
        {
            return new Tag()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name"))
            };
        }

        public void AddTagRelationship(int PostId, int TagId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO PostTag 
                        (PostId, TagId)
                        OUTPUT INSERTED.ID
                        VALUES (@PostId, @TagId)";
                    cmd.Parameters.AddWithValue("@PostId", PostId);
                    cmd.Parameters.AddWithValue("@TagId", TagId);

                    cmd.ExecuteNonQuery();
                }
            }


        }

        public void DeleteTagRelationship(int PostId, int TagId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        DELETE FROM PostTag WHERE PostId = @PostId AND TagId = @TagId";
                    cmd.Parameters.AddWithValue("@PostId", PostId);
                    cmd.Parameters.AddWithValue("@TagId", TagId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
