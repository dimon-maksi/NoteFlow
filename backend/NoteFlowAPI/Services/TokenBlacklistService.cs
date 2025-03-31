namespace NoteFlowAPI.Services;

using System;
using System.Collections.Concurrent;

/// <summary>
/// Service responsible for managing blacklisted JWT tokens.
/// </summary>
public class TokenBlacklistService
{
    private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

    /// <summary>
    /// Adds a token to the blacklist with its expiration time.
    /// </summary>
    /// <param name="token">The token to blacklist.</param>
    /// <param name="expirationTime">When the token naturally expires.</param>
    public void BlacklistToken(string token, DateTime expirationTime)
    {
        _blacklistedTokens.TryAdd(token, expirationTime);

        // Periodically clean up expired tokens from the blacklist
        CleanupExpiredTokens();
    }

    /// <summary>
    /// Checks if a token is blacklisted.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns>True if the token is blacklisted; otherwise, false.</returns>
    public bool IsTokenBlacklisted(string token)
    {
        return _blacklistedTokens.ContainsKey(token);
    }

    /// <summary>
    /// Removes expired tokens from the blacklist.
    /// </summary>
    private void CleanupExpiredTokens()
    {
        var currentTime = DateTime.UtcNow;

        foreach (var token in _blacklistedTokens)
        {
            if (token.Value <= currentTime)
            {
                _blacklistedTokens.TryRemove(token.Key, out _);
            }
        }
    }
}
