namespace OnlineHotelRoomBookingSystem.Web.Infrastructure.ModelBlinders;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Threading.Tasks;
public class DecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        ValueProviderResult valueResut =
                 bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if(valueResut != ValueProviderResult.None && !string.IsNullOrWhiteSpace(valueResut.FirstValue))
        {
            decimal parsedValue = 0m;
            bool binderSucceeded = false;

            try
            {
                string formDecValue = valueResut.FirstValue;
                formDecValue = formDecValue.Replace(",",
                    CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                formDecValue = formDecValue.Replace(".",
                    CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                parsedValue = Convert.ToDecimal(formDecValue);
                binderSucceeded = true;
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex, bindingContext.ModelMetadata);
            }

            if(binderSucceeded)
            {
                bindingContext.Result = ModelBindingResult.Success(parsedValue);
            }
        }

        return Task.CompletedTask;
    }
}
