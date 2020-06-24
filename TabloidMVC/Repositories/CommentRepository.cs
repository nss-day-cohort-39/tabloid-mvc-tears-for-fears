using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class CommentRepository: BaseRepository
    {
        public CommentRepository(IConfiguration config) : base(config) { }
        public List<Comment> GetAllCommentsByPosts(int postId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT 
                            c.Id, 
                            c.PostId, 
                            c.UserProfileId, 
                            c.Content, 
                            c.Subject, 
                            c.CreateDateTime,
                            p.Id as Post_Id,
                            p.Title as PostTitle,
                            u.Id as UserProfile_Id,
                            u.DisplayName
                            FROM Comment c
                            LEFT JOIN Post p ON p.Id = c.PostId
                            LEFT JOIN UserProfile u ON u.Id = c.UserProfileId
                            WHERE c.PostId = @postId
                            ORDER BY c.CreateDateTime DESC";

                    cmd.Parameters.AddWithValue("@postId", postId);
                    var reader = cmd.ExecuteReader();

                    var comments = new List<Comment>();

                    while (reader.Read())
                    {
                        comments.Add(NewCommentFromReader(reader));
                    }

                    reader.Close();

                    return comments;
                }
            }
        }
        private Comment NewCommentFromReader(SqlDataReader reader)
        {
            return new Comment()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),               
                Content = reader.GetString(reader.GetOrdinal("Content")),
                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                Post = new Post
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Post_Id")),
                    Title = reader.GetString(reader.GetOrdinal("PostTitle"))
                },
                UserProfile = new UserProfile()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName"))
                }
            };
        }
        public void AddComment(Comment comment)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Comment(PostId, UserProfileId, Subject, Content, CreateDateTime) 
                                        OUTPUT INSERTED.Id
                                        VALUES(@postId, @userProfileId, @subject,@content, @createDateTime)";
                    cmd.Parameters.AddWithValue("@postId", comment.PostId);
                    cmd.Parameters.AddWithValue("@userProfileId", comment.UserProfileId);
                    cmd.Parameters.AddWithValue("@subject", comment.Subject);
                    cmd.Parameters.AddWithValue("@content", comment.Content);
                    cmd.Parameters.AddWithValue("@createDateTime", comment.CreateDateTime);
                    comment.Id = (int)cmd.ExecuteScalar();
                }
            }
        }
    }

}
