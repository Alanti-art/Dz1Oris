using System.Collections.Generic;

namespace MyHttpServer.Applications
{
    public class Application
    {
        public int Id { get; set; }
        public string Applicant { get; set; }
        public string Direction { get; set; }
        public string Status { get; set; }
    }

    public class ApplicationStore
    {
        private List<Application> applications = new List<Application>();
        private int nextId = 1;

      
        public bool IsValid(string applicant, string direction, string status)
        {
            if (applicant == null)
            {
                return false;
            }

            applicant = applicant.Trim();
            if (applicant.Length < 2 || applicant.Length > 60)
            {
                return false;
            }

            if (direction != "design" && direction != "backend" && direction != "testing")
            {
                return false;
            }

            if (status != "pending" && status != "approved")
            {
                return false;
            }

            return true;
        }

       
        public List<Application> Find(string direction)
        {
            List<Application> result = new List<Application>();

            foreach (Application app in applications)
            {
                if (direction == null || direction == "" || app.Direction == direction)
                {
                    result.Add(app);
                }
            }

            return result;
        }

        public Application GetById(int id)
        {
            foreach (Application app in applications)
            {
                if (app.Id == id)
                {
                    return app;
                }
            }

            return null;
        }

       
        public Application Create(string applicant, string direction)
        {
            if (!IsValid(applicant, direction, "pending"))
            {
                return null;
            }

            Application app = new Application();
            app.Id = nextId;
            app.Applicant = applicant.Trim();
            app.Direction = direction;
            app.Status = "pending";

            nextId++;
            applications.Add(app);
            return app;
        }

     
        public bool Update(int id, string applicant, string direction, string status)
        {
            Application app = GetById(id);
            if (app == null)
            {
                return false;
            }

            if (!IsValid(applicant, direction, status))
            {
                return false;
            }

            app.Applicant = applicant.Trim();
            app.Direction = direction;
            app.Status = status;
            return true;
        }
    }
}
