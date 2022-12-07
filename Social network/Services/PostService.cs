using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PostService
    {
        private readonly IRepository<Post> _PostRepository;

        public PostService(IRepository<Post> repository)
        {
            _PostRepository = repository;
        }
        public virtual List<Post> GetAllPosts()
        {
            return _PostRepository.GetAll();
        }
        public virtual Post GetPostsById(int PostId)
        {
            return _PostRepository.GetById(PostId);
        }
        public virtual void UpdatePost(Post post)
        {
            _PostRepository.Update(post);
        }
        public virtual void DeletePost(int id)
        {
            _PostRepository.Remove(id);
        }
        public virtual void CreatePost(Post user)
        {
            _PostRepository.Create(user);
        }
    }
}
