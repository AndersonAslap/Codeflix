using Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.UnitTests.Domain.Entity.Category;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
        => _categoryTestFixture = categoryTestFixture;
    
    [Fact(DisplayName = nameof(Instatiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instatiate()
    {
        var input = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(input.Name, input.Description);

        category.Should().NotBeNull();
        category.Name.Should().Be(input.Name);
        category.Description.Should().Be(input.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (category.IsActive).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstatiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstatiateWithIsActive(bool isActive)
    {
        var input = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(input.Name, input.Description, isActive);

        category.Should().NotBeNull();
        (category.IsActive).Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        var input = _categoryTestFixture.GetValidCategory();

        Action action =
            () => new DomainEntity.Category(name!, input.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetNameWithLessThan3Characters), parameters: 10)]
    public void InstantiateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        var input = _categoryTestFixture.GetValidCategory();

        Action action =
            () => new DomainEntity.Category(invalidName, input.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at leats 3 characters long");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThan255Characters()
    {
        var input = _categoryTestFixture.GetValidCategory();
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        Action action =
            () => new DomainEntity.Category(invalidName, input.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var input = _categoryTestFixture.GetValidCategory();

        Action action =
            () => new DomainEntity.Category(input.Name, null!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should not be empty or null");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        var input = _categoryTestFixture.GetValidCategory();
        var invalidDescription = String.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());

        Action action =
            () => new DomainEntity.Category(input.Name, invalidDescription);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10.000 characters");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var input = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(input.Name, input.Description, false);
        category.Activate();

        category.Should().NotBeNull();
        (category.IsActive).Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Desactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Desactivate()
    {
        var input = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(input.Name, input.Description, true);
        category.Desactivate();

        category.Should().NotBeNull();
        (category.IsActive).Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var input = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(input.Name, input.Description);

        var inputUpdate = _categoryTestFixture.GetValidCategory();

        category.Update(inputUpdate.Name, inputUpdate.Description);

        category.Should().NotBeNull();
        category.Name.Should().Be(inputUpdate.Name);
        category.Description.Should().Be(inputUpdate.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var input = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(input.Name, input.Description);
        var currentDescription = category.Description;

        var inputUpdate = new
        {
            Name = _categoryTestFixture.GetValidCategoryName(),
        };

        category.Update(inputUpdate.Name);

        category.Should().NotBeNull();
        category.Name.Should().Be(inputUpdate.Name);
        category.Description.Should().Be(currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        var input = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(input.Name, input.Description);

        Action action =
            () => category.Update(name!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    public void UpdateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        var input = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(input.Name, input.Description);

        Action action =
            () => category.Update(invalidName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at leats 3 characters long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThan255Characters()
    {
        var input = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(input.Name, input.Description);

        var invalidName = _categoryTestFixture.Faker.Lorem.Letter(256);

        Action action =
            () => category.Update(invalidName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        var input = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(input.Name, input.Description);

        var invalidDescription = _categoryTestFixture.Faker.Commerce.ProductDescription();

        while(invalidDescription.Length <= 10000)
        {
            invalidDescription = $"{invalidDescription} {_categoryTestFixture.Faker.Commerce.ProductDescription()}";
        }

        Action action =
            () => category.Update(input.Name, invalidDescription);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10.000 characters");
    }

    #region

    // Members Data

    public static IEnumerable<object[]> GetNameWithLessThan3Characters(int numberOfTests = 6)
    {
        var fixture = new CategoryTestFixture();

        for (int i = 0; i < numberOfTests; i++)
        {
            var isOdd = i % 2 == 1;
            yield return new object[] {
                fixture.GetValidCategoryName()[..(isOdd ? 1 : 2)]
            };
        }
    }

    #endregion
}
