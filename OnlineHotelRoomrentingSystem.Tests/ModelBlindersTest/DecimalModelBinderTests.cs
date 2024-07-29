
namespace OnlineHotelRoomrentingSystem.Tests.ModelBlindersTest;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using OnlineHotelRoomBookingSystem.Web.Infrastructure.ModelBlinders;

public class DecimalModelBinderTests
{
    private DefaultModelBindingContext CreateBindingContext(string modelName, string value)
    {
        var valueProvider = new Mock<IValueProvider>();
        valueProvider.Setup(vp => vp.GetValue(modelName))
                     .Returns(new ValueProviderResult(value));

        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var modelMetadata = modelMetadataProvider.GetMetadataForType(typeof(decimal));

        var bindingContext = new DefaultModelBindingContext
        {
            ModelName = modelName,
            ModelState = new ModelStateDictionary(),
            ValueProvider = valueProvider.Object,
            ModelMetadata = modelMetadata
        };

        return bindingContext;
    }

    [Test]
    public async Task BindModelAsync_ValidDecimalString_ShouldBindSuccessfully()
    {
        var binder = new DecimalModelBinder();
        var bindingContext = CreateBindingContext("testDecimal", "123.45");

        await binder.BindModelAsync(bindingContext);

        Assert.That(bindingContext.Result.Model, Is.EqualTo(123.45m));
        Assert.That(bindingContext.ModelState.IsValid, Is.True);
    }

    [Test]
    public async Task BindModelAsync_InvalidDecimalString_ShouldAddModelError()
    {
        var binder = new DecimalModelBinder();
        var bindingContext = CreateBindingContext("testDecimal", "invalidDecimal");

        await binder.BindModelAsync(bindingContext);

        Assert.That(bindingContext.ModelState.IsValid, Is.False);
    }
}
