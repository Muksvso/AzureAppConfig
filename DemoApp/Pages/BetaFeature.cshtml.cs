using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;

namespace DemoApp.Pages
{
    [FeatureGate("BetaFeature")]
    public class BetaFeatureModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
