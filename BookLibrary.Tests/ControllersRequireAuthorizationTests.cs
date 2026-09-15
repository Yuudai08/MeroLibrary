using BookLibrary.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Xunit;

namespace BookLibrary.Tests;

public class ControllersRequireAuthorizationTests
{
    [Theory]
    [InlineData(typeof(BooksController))]
    [InlineData(typeof(AuthorsController))]
    [InlineData(typeof(CategoriesController))]
    public void CatalogControllers_MustHaveAuthorizeAttribute(Type controllerType)
    {
        // Act
        var hasAuthorize = controllerType.GetCustomAttribute<AuthorizeAttribute>() != null;

        // Assert
        Assert.True(hasAuthorize, $"{controllerType.Name} must be decorated with [Authorize] to enforce login.");
    }
}
