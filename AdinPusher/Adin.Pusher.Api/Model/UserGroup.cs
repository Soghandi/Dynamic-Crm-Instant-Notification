using System;
using System.Collections.Concurrent;

namespace Adin.Pusher.Api.Model
{
    public class UserGroup
    {
        public ConcurrentBag<Guid> Users { get; set; }
    }
}