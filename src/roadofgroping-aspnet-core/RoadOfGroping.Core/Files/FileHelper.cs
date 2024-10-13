using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

public class FileHelper
{
    private readonly string _uploadFolder;

    public FileHelper(IWebHostEnvironment env)
    {
        _uploadFolder = Path.Combine(env.WebRootPath, "uploads");
        if (!Directory.Exists(_uploadFolder))
        {
            Directory.CreateDirectory(_uploadFolder);
        }
    }

    // 定义允许的文件扩展名
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx" };
    private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB

    // 通过 IFormFile 保存文件
    public string SaveFile(IFormFile file)
    {
        ValidateFile(file);
        return SaveToFile(file.OpenReadStream(), file.FileName);
    }

    // 通过字节数组保存文件
    public string SaveFile(byte[] fileBytes, string fileName)
    {
        if (fileBytes == null || fileBytes.Length == 0)
        {
            throw new ArgumentException("未选择文件");
        }

        // 检查文件扩展名
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        ValidateFileExtension(extension);

        // 保存文件
        return SaveToFile(new MemoryStream(fileBytes), fileName);
    }

    // 通过流保存文件
    public string SaveFile(Stream fileStream, string fileName)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("未选择文件");
        }

        // 检查文件扩展名
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        ValidateFileExtension(extension);

        return SaveToFile(fileStream, fileName);
    }

    // 从 HttpContent 保存文件
    public async Task<string> SaveFileAsync(HttpContent content, string fileName)
    {
        if (content == null)
        {
            throw new ArgumentException("未选择文件");
        }

        var fileStream = await content.ReadAsStreamAsync();

        // 检查文件扩展名
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        ValidateFileExtension(extension);

        return SaveToFile(fileStream, fileName);
    }

    // 保存文件到指定路径
    private string SaveToFile(Stream fileStream, string fileName)
    {
        var filePath = Path.Combine(_uploadFolder, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            fileStream.CopyTo(stream);
        }
        return fileName;
    }

    // 验证文件
    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("未选择文件");
        }

        if (file.Length > _maxFileSize)
        {
            throw new ArgumentException("文件大小超过限制");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        ValidateFileExtension(extension);
    }

    // 验证文件扩展名
    private void ValidateFileExtension(string extension)
    {
        if (Array.IndexOf(_allowedExtensions, extension) < 0)
        {
            throw new ArgumentException("不允许的文件类型");
        }
    }

    // 获取文件路径
    public string GetFilePath(string filename)
    {
        return Path.Combine(_uploadFolder, filename);
    }

    // 删除文件
    public void DeleteFile(string filename)
    {
        var filePath = GetFilePath(filename);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
