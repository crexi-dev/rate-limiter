using RateLimiter.Entity.Entities;
using RateLimiter.Entity.ViewModels.Department;
using RateLimiter.Repository;
using RateLimiter.Service.Interfaces;
using RateLimiter.Service.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Service.Services
{
    public class DepartmentService : IDepartmentService
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        /// <summary>
        /// Delete an existing department usint UOW approach
        /// </summary>
        /// <param name="departmentId">Deparment Id </param>
        /// <returns>Department View Model</returns>
        public DepartmentViewModel DeleteDepartment(int departmentId)
        {
            var departmentViewModel = new DepartmentViewModel();
            try
            {
                var departmentDb = unitOfWork.DepartmentRepository.GetByID(departmentId);

                if (departmentDb == null)
                {
                    departmentViewModel.ErrorMessage = DEPARTMENT.DOES_NOT_EXIST;
                }
                else
                {
                    unitOfWork.DepartmentRepository.Delete(departmentId);
                    unitOfWork.Save();
                    departmentViewModel.SuccessMessage = DEPARTMENT.DELETE_MESSAGE;
                }
            }
            catch (Exception ex)
            {
                departmentViewModel.ErrorMessage = ex.Message;
            }

            return departmentViewModel;

        }

        /// <summary>
        /// Get an existing department usint UOW approach
        /// </summary>
        /// <param name="departmentId">Deparment Id </param>
        /// <returns>Department View Model</returns>

        public DepartmentViewModel GetDepartmentById(int departmentId)
        {
            var departmentViewModel = new DepartmentViewModel();
            try
            {
                var departmentDb = unitOfWork.DepartmentRepository.GetByID(departmentId);

                if (departmentDb == null)
                {
                    departmentViewModel.ErrorMessage = DEPARTMENT.DOES_NOT_EXIST;
                }
                else
                {
                    //TODO : prefer to use auto mapper thant do it manually.
                    departmentViewModel = MapEntityToVM(departmentDb);
                }
            }
            catch (Exception ex)
            {
                departmentViewModel.ErrorMessage = ex.Message;
            }

            return departmentViewModel;
        }

        /// <summary>
        /// Get List Of Departments using UOW Approach
        /// </summary>
        /// <returns>List of Departmetns</returns>
        public DepartmentListViewModel GetDepartments()
        {
            var departmentListView = new DepartmentListViewModel();
            try
            {
                var departments = unitOfWork.DepartmentRepository.Get();

                var deparmentsVM = from d in departments
                                  select new DepartmentViewModel()
                                  {
                                      DepartmentId = d.DepartmentId,
                                      Name = d.Name 
                                  };

                departmentListView.Departments = deparmentsVM.ToList();
                departmentListView.SuccessMessage = DEPARTMENT.LIST_MESSAGE;
            }
            catch (Exception ex)
            {
                departmentListView.ErrorMessage = ex.Message;
            }

            return departmentListView; 
        }

        /// <summary>
        ///  Insert a new deparment entity using UOW  approach.
        /// </summary>
        /// <param name="department">Deparment View Model</param>
        /// <returns>Deparment View Model</returns>
        public DepartmentViewModel InsertDepartment(DepartmentInsertViewModel department)
        {
            var departmentVM = new DepartmentViewModel();
            try
            {

                /// TODO: Perform any validation on the view model before to insert the record in the database.
                /// 
                /// we can use automapper instead to do ti manually
                var deparmentDb = new Department()
                {                   
                    Name = department.Name
                };

                unitOfWork.DepartmentRepository.Insert(deparmentDb);
                unitOfWork.Save();
                 
                departmentVM.SuccessMessage = DEPARTMENT.INSERT_MESSAGE;
            }
            catch (Exception ex)
            {
                departmentVM.ErrorMessage = ex.Message;
            }
            return departmentVM;

        }

        private DepartmentViewModel MapEntityToVM (Department entity)
        {
            return new DepartmentViewModel()
            {
                DepartmentId = entity.DepartmentId,
                Name = entity.Name
            };
        }
    }
}
