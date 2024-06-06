using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Services.Redis;
public class RedisLock : IDisposable
{
    private readonly IDatabase _database;
    private readonly string _lockKey;
    private readonly string _lockValue;
    private readonly TimeSpan _expiry;

    public RedisLock(IDatabase database, string lockKey, string lockValue, TimeSpan expiry)
    {
        _database = database;
        _lockKey = lockKey;
        _lockValue = lockValue;
        _expiry = expiry;
    }

    //setup khóa vào Redis
    public bool AcquireLock()
    {
        return _database.StringSet(_lockKey, _lockValue, _expiry, When.NotExists);
    }

    //trả khóa sau khi done
    public void ReleaseLock()
    {
        var script = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";
        _database.ScriptEvaluate(script, new RedisKey[] { _lockKey }, new RedisValue[] { _lockValue });
    }
    public void Dispose()
    {
        ReleaseLock();
    }
}
