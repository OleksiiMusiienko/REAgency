using REAgency.BLL.DTO.Persons;

namespace REAgency.Models
{
    public class EmployeesPageViewModel
    {
        public IEnumerable<EmployeeDTO> Employees { get; }
        public PageViewModel PageViewModel { get; }
        public string typeOfAction { get; set; }
        public EmployeesPageViewModel(IEnumerable<EmployeeDTO> employees, PageViewModel viewModel)
        {
            Employees = employees;
            PageViewModel = viewModel;

        }
    }
}
