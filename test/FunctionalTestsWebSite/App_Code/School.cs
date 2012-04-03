using System;
using System.Web;
using System.Web.Caching;
using System.Data;
using System.Collections.Generic;
using System.Collections;

namespace QAScenarios
{
    public class School
    {
        private SchoolDataSet _schoolDS;
        
        public School()
        {
            if (HttpContext.Current.Session["dataset"] == null)
            {
                InitializeDataSet();
            }
            else
            {
                _schoolDS = (SchoolDataSet)HttpContext.Current.Session["dataset"];
            }
        }

        private void InitializeDataSet()
        {
            _schoolDS = new SchoolDataSet();

            //build the books table
            for (int i = 0; i < 20; i++)
            {
                _schoolDS.Books.AddBooksRow(i, "Book Title #" + i.ToString(), "Author_" + i.ToString());
            }

            //build the courses table
            for (int i = 0; i < 20; i++)
            {
                SchoolDataSet.CoursesRow course = _schoolDS.Courses.NewCoursesRow();
                course.Course_Id = i;
                course.Name = "Course Name #" + i.ToString();
                course.Description = "Description_" + i.ToString();
                course.Details1 = "Details for Course #" + i.ToString();
                course.Details2 = "This course consists of " + i.ToString() + " classes";
                course.Details3 = "The passing grade is " + i.ToString();
                course.Grade1 = 0;
                course.Grade2 = 0;
                course.Grade3 = 0;
                course.Final = 0;
                course.Book_Id = i;
                course.Required = false;
                _schoolDS.Courses.AddCoursesRow(course);
            }

            SaveDataSet();
        }

        /// <summary>
        /// Create a dataset with the given number of rows.
        /// </summary>
        public School(int noOfRows)
        {
            InitializeDataSet(noOfRows);
        }

        /// <summary>
        /// Initialize a dataset with the given number of rows.
        /// </summary>
        private void InitializeDataSet(int count)
        {
                _schoolDS = new SchoolDataSet();

                //build the books table
                for (int i = 0; i < count; i++)
                {
                    _schoolDS.Books.AddBooksRow(i, "Book Title #" + i.ToString(), "Author_" + i.ToString());
                }

                //build the courses table
                for (int i = 0; i < count; i++)
                {
                    SchoolDataSet.CoursesRow course = _schoolDS.Courses.NewCoursesRow();
                    course.Course_Id = i;
                    course.Name = "Course Name #" + i.ToString();
                    course.Description = "Description_" + i.ToString();
                    course.Details1 = "Details for Course #" + i.ToString();
                    course.Details2 = "This course consists of " + i.ToString() + " classes";
                    course.Details3 = "The passing grade is " + i.ToString();
                    course.Grade1 = 0;
                    course.Grade2 = 0;
                    course.Grade3 = 0;
                    course.Final = 0;
                    course.Book_Id = i;
                    course.Required = false;
                    _schoolDS.Courses.AddCoursesRow(course);
                }

                SaveDataSet();
        }

        private void SaveDataSet()
        {
            HttpContext.Current.Session["dataset"] = _schoolDS;
        }

        public DataView GetCourses(string sortColumn)
        {
            _schoolDS.Courses.DefaultView.Sort = sortColumn;
            return _schoolDS.Courses.DefaultView;
        }

        public SchoolDataSet.CoursesRow GetCourseById(int Course_Id)
        {
            return _schoolDS.Courses.FindByCourse_Id(Course_Id);
        }

        public SchoolDataSet.BooksRow GetBookForCourse(int Course_Id)
        {
            return _schoolDS.Courses.FindByCourse_Id(Course_Id).BooksRow;
        }
        
        /// <summary>
        ///  UpdateCourseGrades with five parameters
        /// </summary>
        public void UpdateCourseGrades(int Course_Id, int Grade1, int Grade2, int Grade3, int Final)
        {
            SchoolDataSet.CoursesRow course = _schoolDS.Courses.FindByCourse_Id(Course_Id);
            course.Grade1 = Grade1;
            course.Grade2 = Grade2;
            course.Grade3 = Grade3;
            course.Final = Final;
            SaveDataSet();
        }
        

        public void UpdateCourseGrades(int Course_Id, bool Required, int Grade1, int Grade2, int Grade3, int Final)
        {
            SchoolDataSet.CoursesRow course = _schoolDS.Courses.FindByCourse_Id(Course_Id);
            course.Grade1 = Grade1;
            course.Grade2 = Grade2;
            course.Grade3 = Grade3;
            course.Final = Final;
            course.Required = Required;
            SaveDataSet();
        }

        public void UpdateCourseDetails(int Course_Id, string Name, string Description, bool Required, string Details1, string Details2, string Details3, int Book_Id)
        {
            SchoolDataSet.CoursesRow course = _schoolDS.Courses.FindByCourse_Id(Course_Id);
            course.Name = Name;
            course.Description = Description;
            course.Required = Required;
            course.Details1 = Details1;
            course.Details2 = Details2;
            course.Details3 = Details3;
            course.Book_Id = Book_Id;
            SaveDataSet();
        }

        public void AddExtraPoint(int Course_Id)
        {
            SchoolDataSet.CoursesRow course = _schoolDS.Courses.FindByCourse_Id(Course_Id);
            course.Final += 1000;
            SaveDataSet();
        }

		public void RemoveExtraPoint(int Course_Id)
		{
			SchoolDataSet.CoursesRow course = _schoolDS.Courses.FindByCourse_Id(Course_Id);
			course.Final -= 1000;
			SaveDataSet();
		}

        public void SetRequiredCourse(int Course_Id, bool Required)
        {
            SchoolDataSet.CoursesRow course = _schoolDS.Courses.FindByCourse_Id(Course_Id);
            course.Required = Required;
            SaveDataSet();
        }

        public void DeleteCourse(int Course_Id)
        {
            _schoolDS.Courses.RemoveCoursesRow(_schoolDS.Courses.FindByCourse_Id(Course_Id));
            SaveDataSet();
        }

        public List<SchoolDataSet.CoursesRow> RequiredClasses()
        {
            List<SchoolDataSet.CoursesRow> courses = new List<SchoolDataSet.CoursesRow>();

            foreach (SchoolDataSet.CoursesRow c in _schoolDS.Courses.Rows)
            {
                if (c.Required)
                {
                    courses.Add(c);
                }
            }

            return courses;
        }

        public void InsertCourse(int Course_Id, string Name, string Description, bool Required, string Details1, string Details2, string Details3, int Book_Id)
        {
            SchoolDataSet.CoursesRow newCourse = _schoolDS.Courses.NewCoursesRow();
            newCourse.Course_Id = Course_Id;
            newCourse.Name = Name;
            newCourse.Description = Description;
            newCourse.Required = Required;
            newCourse.Grade1 = 0;
            newCourse.Grade2 = 0;
            newCourse.Grade3 = 0;
            newCourse.Final = 0;
            newCourse.Details1 = Details1;
            newCourse.Details2 = Details2;
            newCourse.Details3 = Details3;
            newCourse.Book_Id = Book_Id;

            _schoolDS.Courses.AddCoursesRow(newCourse);
            SaveDataSet();
        }
    }
}