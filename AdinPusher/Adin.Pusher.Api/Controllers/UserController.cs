using System;
using System.Web.Http;
using Adin.Pusher.Api.Cache;
using Adin.Pusher.Api.Model;

namespace Adin.Pusher.Api.Controllers
{
    public class UserController : ApiController
    {

        [HttpGet]
        public int JoinGroup(Guid userId, string groupName)
        {
            return UserCache.Instance.JoinGroup(userId, groupName);
        }

        [HttpPost]
        public int JoinGroups([FromBody]JoinData data)
        {
            return UserCache.Instance.JoinGroups(data);
        }

        [HttpGet]
        private int RemoveUser(Guid key)
        {
            return UserCache.Instance.RemoveUser(key);
        }
    }
}
