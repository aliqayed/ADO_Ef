using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ADO_Ef
{
    class Program
    {
        static void Main(string[] args)
        {
            #region ADO.NET_iqueryable_vs_ienumerable
            // iquerable  vs ienumerable
            // iqueryable is a query that is not executed yet ( memory )
            // ienumerable is a query that is executed ( db )

            SchoolEntities1 context = new SchoolEntities1();

            context.Database.Log = log => Debug.WriteLine(log);

            var employees = context.EmployeeS.Where(emp => emp.ID > 0);
            // diveration from iqueryable to ienumerable
            // it will return all employees from DB => ienumerable list of employees
            var Query = from emp in context.EmployeeS
                        where emp.ID > 0
                        select emp;
            foreach (var q in Query)
            {
                Debug.WriteLine(q.Name);
                Console.WriteLine(q.Name);
            }

            // it will return all employees from DB => qureaable list of employees
            foreach (var employee in employees)
            {
                Debug.WriteLine(employee.Department + " " + employee.Name);
                Console.WriteLine(employee.Department + " " + employee.Name);
            }

            var departments = context.DepartmentS1.ToList().Where(dep => dep.ID > 0);
            var Query2 = (from dep in context.DepartmentS1
                          where dep.ID > 0
                          select dep).ToList();// data in memory
            // it will return all departments from DB => list of departments ienumberable
            // it will return all departments from DB => list of departments ienumberable
            foreach (var department in departments)
            {
                Debug.WriteLine(department.Name);
                Console.WriteLine(department.Name + " " + department.EmployeeS.Count);
            }

            // conver from ienumerable to iqueryable
            var departments2 = context.DepartmentS1.AsQueryable().Where(dep => dep.ID > 0);
            // it will return all departments from DB => list of departments iqueryable
            foreach (var department in departments2)
            {
                Debug.WriteLine(department.Name);
                Console.WriteLine(department.Name + " " + department.EmployeeS.Count);
            }
            // convert from iqueryable to ienumerable
            var departments3 = context.DepartmentS1.AsQueryable().Where(dep => dep.ID > 0).AsEnumerable();
            // it will return all departments from DB => list of departments ienumerable
            foreach (var department in departments3)
            {
                Debug.WriteLine(department.Name);
                Console.WriteLine(department.Name + " " + department.EmployeeS.Count);
            }
            #endregion

            #region ADO.NET_Lazy_Eager_Explicit_Loading
            //-----------------------------
            //var empGenric = context.Set<T>.ToList();
            var empFrist = context.Departments.First();
            var empSingle = context.Departments.Single();
            var empFirstOrDefault = context.Departments.FirstOrDefault();
            var empSingleOrDefault = context.Departments.SingleOrDefault();
            // emp When Accessing the first element, the query will be executed
            // lazy loading ..... 4 is method that will be executed in the database
            // lazy loading ..... is the default behavior of entity framework
            // by not loading the related entities until you access them (navigation property) by default virtual 
            //foreach (var e in emp.Id)
            //{
            //    Debug.WriteLine(e.Name);
            //    Console.WriteLine(e.Name);
            //}

            var dept = context.Departments.Find();
            // eager loading
            // explicit loading
            // lazy loading
            dept.Name = "ITI";
            dept.Projects.Add(new Project() { Name = "ITI", deparmentId = 10 });
            context.SaveChanges();
            #endregion

            #region ADO.NET_Tracking_AsNoTracking
            //-----------------------------
            /* tracking
            // context.ChangeTracker.Entries() => get all entities that are being tracked by the context
            // context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added) => get all entities that are being tracked by the context and added
            // context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified) => get all entities that are being tracked by the context and modified
            // context.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted) => get all entities that are being tracked by the context and deleted
            // context.ChangeTracker.Entries().Where(e => e.State == EntityState.Unchanged) => get all entities that are being tracked by the context and unchanged
            // context.ChangeTracker.Entries().Where(e => e.State == EntityState.Detached) => get all entities that are being tracked by the context and detached
            */
            var dept1 = context.Departments.Find(1);
            dept1.Name = "ITI";
            var dept2 = context.Departments.Find(1);
            dept2.Name = "Ali";
            var dept3 = context.Departments.Find(1);
            dept3.Name = "Mohamed";
            var dept4 = context.Departments.Find(1);
            dept4.Name = "Ahmed";
            var dept5 = context.Departments.Find(1);
            dept5.Name = "Mahmoud";
            var dept6 = context.Departments.Find(1);
            dept6.Name = "Hassan";
            context.SaveChanges();
            /*Any changes that you make to the entity will be
            //tracked by the context and will be saved to the database when you call the SaveChanges() method.
            // Any object that is being tracked by the context is called a tracked entity.
            // The context will keep track of all the changes that you make to the tracked entities.
            // class that is being tracked by the context is called a tracked entity.
            // The context will keep track of all the changes that you make to the tracked entities.
            // tracking is the default behavior of entity framework , you can disable it by using AsNoTracking() method 
            // cross cutting concern => tracking , logging , caching , validation , security , exception handling , transaction management , performance , concurrency , scalability
            // pros of tracking => easy to use , easy to implement , easy to maintain , easy to test , easy to debug , easy to extend , easy to scale , easy to deploy , easy to monitor  , easy to manage  
            // change in db astracking  any object that is being tracked by the context is called a tracked entity*/

            context.Entry(dept1).State = System.Data.Entity.EntityState.Detached;  // it will not be tracked by the context
                                                                                   // to change the state of the entity to detached so it will not be tracked by the context

            context.Set<Department>().Attach(dept1);
            //=> it will be tracked by the context unmodified and it will not be saved to the database
            context.Entry(dept2).State = System.Data.Entity.EntityState.Unchanged;
            // it will be tracked by the context but it will not be saved to the database
            context.Entry(dept3).State = System.Data.Entity.EntityState.Modified;
            // it will be tracked by the context and it will be saved to the database
            context.Entry(dept4).State = System.Data.Entity.EntityState.Added;
            // it will be tracked by the context and it will be saved to the database
            // if data read only use AsNoTracking() method when you read data from the database and not change it
            #endregion

            #region ADO.NET_baulk_insert_update_delete_modifed
            //-----------------------------
            // baulk insert
            // baulk update
            var d = context.Departments.Single(Single => Single.Id == 1);
            // select * from department where id=1  form the database
            d = context.Departments.Find(1);
            // select * from department where id=1  form memory then from cache then from database

            // baulk insert
            d = new Department() { Id = 1, Name = "ITI" };
            //to insert the department in the database
            context.Departments.Add(d);
            //to insert the department in the database range
            d.Projects =
            new List<Project>()
            {
              new Project(){ },
                new Project() { Name = "Qayed" }, new Project() { Name = "Essam" }
            };
            context.Departments.AddRange((IEnumerable<Department>)d);

            context.SaveChanges();
            // baulk delete
            context.Departments.Remove(d);
            //to delete the department in the database
            context.SaveChanges();
            // execute in tranzaction
            // be tracked by the context and will be saved to the database when you call the SaveChanges() method.
            var enp = new Department()
            {
                Id = 1,
                Name = "ITI"
                //new List<Project> p
                //{ 
                //} 
            };
            #endregion

            #region ADO.NET_Validation
            //-----------------------------
            // validation
            // data annotation
            // fluent api
            // custom validation
            // data annotation
            // [Required] => the property is required
            // [StringLength(50)] => the property should have a maximum length of 50 characters
            // [Range(1, 100)] => the property should have a value between 1 and 100
            // [RegularExpression(@"\d{3}-\d{2}-\d{4}")] => the property should have a value that matches the regular expression
            // [EmailAddress] => the property should have a valid email address
            // [Phone] => the property should have a valid phone number
            // [Url] => the property should have a valid URL
            // [CreditCard] => the property should have a valid credit card number
            // [Compare("Password")] => the property should have the same value as the Password property
            // [DataType(DataType.Date)] => the property should have a valid date
            // [Display(Name = "Full Name")] => the property should have a display name of Full Name
            // [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] => the property should have a display format of yyyy-MM-dd 
            // [Editable(false)] => the property should not be editable
            // [ReadOnly(true)] => the property should be read-only
            // [ScaffoldColumn(false)] => the property should not be scaffolded
            // [Key] => the property should be a primary key
            // [ForeignKey("Department")] => the property should be a foreign key
            // [InverseProperty("Department")] => the property should be an inverse property
            // [NotMapped] => the property should not be mapped to the database
            // [Table("Department")] => the property should be mapped to the Department table
            // [Column("Name")] => the property should be mapped to the Name column
            // [Index("Name", IsUnique = true)] => the property should have a unique index
            // [Timestamp] => the property should have a timestamp
            // [ConcurrencyCheck] => the property should have a concurrency check
            // [DatabaseGenerated(DatabaseGeneratedOption.Identity)] => the property should have an identity column
            // [DatabaseGenerated(DatabaseGeneratedOption.Computed)] => the property should have a computed column
            // [DatabaseGenerated(DatabaseGeneratedOption.None)] => the property should have a none column
            // [DatabaseGenerated(DatabaseGeneratedOption.Computed)] => the property should have a computed column
            // [DatabaseGenerated(DatabaseGeneratedOption.None)] => the property should have a none column
            // [DatabaseGenerated(DatabaseGeneratedOption.Computed)] => the property should have a computed column
            // [DatabaseGenerated(DatabaseGeneratedOption.None)] => the property should have a none column
            #endregion
            #region update_baulek_concurrency
            // Update baulek
            var em = context.EmployeeS.Find(5);
            em.Name += "ITI";
            // update the name of the employee
            // concurrency check => the property should have a concurrency check
            context.SaveChanges();
            #endregion
            #region ADO.NET_Concurrency
            //-----------------------------
            // concurrency
            var dept7 = context.Departments.Find(1);
            // optimistic concurrency
            dept7.Name = "ITI";
            // pessimistic concurrency
            context.SaveChanges();
            // concurrency token
            var dept8 = context.Departments.Find(1);
            // timestamp

            // rowversion

            // concurrency check

            #endregion
        }
    }
}
