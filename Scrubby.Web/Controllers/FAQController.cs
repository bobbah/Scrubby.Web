using System.Threading.Tasks;
using Markdig;
using Markdig.Prism;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Models;

namespace Scrubby.Web.Controllers;

public class FAQController : Controller
{
    public async Task<IActionResult> Index()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UsePrism()
            .UseBootstrap()
            .Build();

        var page = Markdown.ToHtml(await GetFAQMarkdown(), pipeline);

        return View("FAQ", new FAQViewModel { Raw = page });
    }

    [ResponseCache(Duration = 300)]
    private static async Task<string> GetFAQMarkdown()
    {
        return await System.IO.File.ReadAllTextAsync(@"Views/FAQ/FAQ.md");
    }

    public IActionResult SecurityPolicy()
    {
        return View("SecurityPolicy");
    }
}