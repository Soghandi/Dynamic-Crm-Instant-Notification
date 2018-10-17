using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Web;
using Adin.Pusher.Api.Controllers;
using Adin.Pusher.Api.Model;
using Adin.Pusher.Domain.Logger;

namespace Adin.Pusher.Api.Cache
{
    public class MobileUserCache
    {

        private MobileUserCache()
        {

        }

        private static MobileUserCache _userCache = new MobileUserCache();

        public static MobileUserCache Instance
        {
            get { return _userCache; }
        }

        private static readonly ConcurrentDictionary<Guid, WebSocket> _users = new ConcurrentDictionary<Guid, WebSocket>();

        private static readonly ConcurrentDictionary<string, UserGroup> _userGroups = new ConcurrentDictionary<string, UserGroup>();


        public int JoinGroup(Guid userId, string groupName)
        {
            var userGroup = _userGroups.GetOrAdd(groupName, new UserGroup()
            {
                Users = new ConcurrentBag<Guid>()
            });
            if (!userGroup.Users.Contains(userId))
            {
                userGroup.Users.Add(userId);
                return 1;
            }

            return 0;
        }

        public int JoinGroups(JoinData data)
        {
            Log.Instance.Error("groups: " + data.GroupNames);
            string[] groups = data.GroupNames.Split(',');
            foreach (var grp in groups)
            {
                Log.Instance.Info("group: " + grp);
            }
            foreach (var groupName in groups)
            {
                var userGroup = _userGroups.GetOrAdd(groupName, new UserGroup()
                {
                    Users = new ConcurrentBag<Guid>()
                });
                if (!userGroup.Users.Contains(data.UserId))
                {
                    userGroup.Users.Add(data.UserId);
                    Log.Instance.Info(data.UserId + " Added To " + groupName);
                }
            }
            return groups.Length;
        }

        public int RemoveUser(Guid key)
        {
            if (_users.ContainsKey(key))
            {
                WebSocket ows;
                _users.TryRemove(key, out ows);
                foreach (var userGroup in _userGroups)
                {
                    if (userGroup.Value.Users.Contains(key))
                    {
                        Guid og;
                        userGroup.Value.Users.TryTake(out og);
                        return 1;
                    }
                }
            }
            return 0;
        }

        public ConcurrentDictionary<Guid, WebSocket> Users()
        {
            return _users;
        }

        public ConcurrentDictionary<string, UserGroup> UserGroups()
        {
            return _userGroups;
        }
    }
}