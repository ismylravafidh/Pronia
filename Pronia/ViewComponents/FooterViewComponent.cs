namespace Pronia.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        AppDbContext _context;

        public FooterViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var setting = _context.Setting.ToDictionary(x => x.Key, x => x.Value);
            return View(setting);
        }
    }
}
