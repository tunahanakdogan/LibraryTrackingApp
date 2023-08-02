using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
namespace LibraryTrackingApp
{
    public partial class Form1 : Form
    {
        SqlConnection sqlConnection = CreateSQLConnection();
        public static SqlConnection CreateSQLConnection()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection("Server=.\\SQLEXPRESS;Database=Books;Trusted_Connection=True;");
                return sqlConnection;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public void DeleteRecord() 
        {
            string bookNameToDelete = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            Console.WriteLine(bookNameToDelete);
        }
        public Author GetAuthorByID(int authorID)
        {
            return sqlConnection.Query<Author>("select * from Author where AuthorID = @AuthorID", new { AuthorID = authorID }).FirstOrDefault();
        }
        public Author GetAuthorByName(string authorName)
        {
            return sqlConnection.Query<Author>("select * from Author where AuthorName = @AuthorName", new {AuthorName = authorName}).FirstOrDefault();
        }
        private void AddBook(string authorName)
        {
           Author authorToAdd = GetAuthorByName(authorName);
           if(authorToAdd == null)
            {
               sqlConnection.Execute("insert into Author(AuthorName) values(@AuthorName)", new { AuthorName = authorName });
               authorToAdd = GetAuthorByName(authorName);
           }
           sqlConnection.Execute("insert into Book(BookName,AuthorID) values(@BookName,@AuthorID)", new { BookName = textBox1.Text, AuthorID = authorToAdd.AuthorID });
        }
        private void FillGrid()
        {
            dataGridView1.Rows.Clear();
            //form kapanırken connectionı kapat
            List<Book> books = sqlConnection.Query<Book>(
                   "select * from Book").ToList();
            dataGridView1.ColumnCount = 3;

            dataGridView1.Columns[0].Name = "Read";
            dataGridView1.Columns[1].Name = "BookName";
            dataGridView1.Columns[2].Name = "AuthorName";
            
            foreach (var book in books)
            {
                if (book.Read == true) dataGridView1.Rows.Add("Yes", book.BookName, GetAuthorByID(book.AuthorID).AuthorName);
                else dataGridView1.Rows.Add("No", book.BookName, book.AuthorID);

            }
        }
        public Form1()
        {
            InitializeComponent();
            FillGrid();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddBook(textBox2.Text);
            FillGrid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeleteRecord();
        }
    }
}
