﻿namespace Application.Exceptions
{
    /// <summary>
    /// TODO: This should be a 400 Bad Request exception
    /// </summary>
    public class RepositoryAccessException : Exception
    {
        public RepositoryAccessException()
        {
        }

        public RepositoryAccessException(string message)
            : base(message)
        {
        }

        public RepositoryAccessException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
