using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN221PE_SP24_TrialTest_SE160233_Repo;
using PRN221PE_SP24_TrialTest_SE160233_Repo.Repo;

namespace PRN221PE_SP24_TrialTest_SE160233_RazorPage.Pages.EyeGlassPage
{
    public class EditModel : PageModel
    {
        protected IUnitOfWork _unitOfWork;

        public EditModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Eyeglass Eyeglass { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!HttpContext.Session.GetString("UserRole").Equals("ADMIN"))
            {
                TempData["ErrorMessage"] = "You do not have permission to access this page.";
                return RedirectToPage("/eyeglasspage/index");
            }
            var eyeglass = _unitOfWork.EyeglassRepository.GetById(id);
            if (eyeglass == null)
            {
                return NotFound();
            }
            Eyeglass = eyeglass;
            ViewData["LensTypeId"] = new SelectList(_unitOfWork.LensTypeRepository.GetAll(), "LensTypeId", "LensTypeId");
            ViewData["LensTypeName"] = new SelectList(_unitOfWork.LensTypeRepository.GetAll(), "LensTypeName", "LensTypeName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (Eyeglass.Quantity < 0 || Eyeglass.Quantity > 999)
            {
                ModelState.AddModelError("Eyeglass.Quantity", "Quantity must be >=0 and <= 999");
                //ViewData["LensTypeId"] = new SelectList(_unitOfWork.LensTypeRepository.GetAll(), "LensTypeId", "LensTypeId");
                ViewData["LensTypeName"] = new SelectList(_unitOfWork.LensTypeRepository.GetAll(), "LensTypeName", "LensTypeName");
                return Page();
            }

            if (Eyeglass.EyeglassesName.Length < 10 || !Eyeglass.EyeglassesName.Split(' ').All(word => char.IsUpper(word[0]) || char.IsDigit(word[0]) || new[] { '@', '#', '$', '&', '(', ')' }.Contains(word[0])))
            {
                ModelState.AddModelError("Eyeglass.EyeglassesName", "Eyeglasses name must be at least 11 characters, each word must begin with a capital letter, a number, or a special character such as @, #, $, &, (, )");
                //ViewData["LensTypeId"] = new SelectList(_unitOfWork.LensTypeRepository.GetAll(), "LensTypeId", "LensTypeId");
                ViewData["LensTypeName"] = new SelectList(_unitOfWork.LensTypeRepository.GetAll(), "LensTypeName", "LensTypeName");
                return Page();
            }

            _unitOfWork.EyeglassRepository.Update(Eyeglass);
            _unitOfWork.SaveChanges();
            return RedirectToPage("./Index");
        }

    }
}
