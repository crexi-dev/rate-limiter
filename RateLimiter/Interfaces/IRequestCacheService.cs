using System;

public interface IRequestCacheService
{
    T GetData<T>(string key);

    void SetData<T>(string key, T value, TimeSpan expirationTime);
}