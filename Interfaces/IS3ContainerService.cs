using Microsoft.AspNetCore.Mvc;

namespace IndividualWorkAPI.Interfaces;

public interface IS3ContainerService
{
    Task<string> GeneratePresignedPutUrlAsync(string key, string contentType, int expirationMinutes);
    Task<string> GeneratePresignedGetUrlAsync(string key, int expirationMinutes = 60);
    Task<bool> DeleteObjectAsync(string key);
}