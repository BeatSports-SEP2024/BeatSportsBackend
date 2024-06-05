using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Exceptions;
public class FireBaseStorageException : Exception
{
    // Thêm một trường để lưu trữ thông tin thêm, ví dụ mã lỗi từ Firebase
    public string ErrorCode { get; }

    // Constructor mặc định
    public FireBaseStorageException()
    {
    }

    // Constructor cho phép truyền thông điệp lỗi
    public FireBaseStorageException(string message)
        : base(message)
    {
    }

    // Constructor cho phép truyền thông điệp và ngoại lệ gốc
    public FireBaseStorageException(string message, Exception inner)
        : base(message, inner)
    {
    }

    // Constructor cho phép truyền thông điệp, mã lỗi và ngoại lệ gốc
    public FireBaseStorageException(string message, string errorCode, Exception inner)
        : base(message, inner)
    {
        ErrorCode = errorCode;
    }

    // Constructor cho phép truyền thông điệp và mã lỗi
    public FireBaseStorageException(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }
}