using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Provides data access methods for managing users in the TuneSpace application.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetUserByIdAsync(string id);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="name">The username to search for.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetUserByNameAsync(string name);

    /// <summary>
    /// Searches for users whose names match the specified search term.
    /// </summary>
    /// <param name="name">The search term to match against usernames.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of user entities whose names match the search criteria.</returns>
    Task<List<User>> SearchByNameAsync(string name);

    /// <summary>
    /// Creates a new user in the database with the specified password.
    /// </summary>
    /// <param name="user">The user entity to insert.</param>
    /// <param name="password">The password for the new user (to be hashed during storage).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertUserAsync(User user, string password);

    /// <summary>
    /// Updates an existing user's information in the database.
    /// </summary>
    /// <param name="user">The user entity with updated information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateUserAsync(User user);
}
