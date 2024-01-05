using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chat.Pages;

public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Channels { get; set; }
}