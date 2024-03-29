﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
namespace LibraryTrackingApp
{
    //Todo 
    //Gridden editleyince sql güncellenmiyor onu değiştir. 


    public partial class Form1 : Form
    {
        readonly SqlConnection sqlConnection = CreateSQLConnection();
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

        public  void DeleteRecord() 
        {
            try
            {
                string bookNameToDelete = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                string authorNameToDelete = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                GetAuthorByName(authorNameToDelete);
                
                if (GetBookAuthors(authorNameToDelete) == 1 )
                {
                    sqlConnection.Execute("delete from Author where AuthorName = @AuthorName", new { AuthorName = authorNameToDelete });
                }
                sqlConnection.Execute("delete from Book where BookName = @BookName", new { BookName = bookNameToDelete });
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during deletion: " + ex.ToString());
                throw;
            }
        }
        public Author GetAuthorByID(int authorID)
        {
            return sqlConnection.Query<Author>("select * from Author where AuthorID = @AuthorID", new { AuthorID = authorID }).FirstOrDefault();
        }
        public Author GetAuthorByName(string authorName)
        {
            return sqlConnection.Query<Author>("select * from Author where AuthorName = @AuthorName", new {AuthorName = authorName}).FirstOrDefault();
        }
        public int GetBookAuthors(string authorName)
        {
            return sqlConnection.Query<int>("select Count(author.AuthorName) from book inner join author on Book.AuthorID = Author.AuthorID").First();
        }
        private void AddBook(string authorName, bool read)
        {
           Author authorToAdd = GetAuthorByName(authorName);
           if(authorToAdd == null)
            {
               sqlConnection.Execute("insert into Author(AuthorName) values(@AuthorName)", new { AuthorName = authorName });
               authorToAdd = GetAuthorByName(authorName);
           }
           sqlConnection.Execute("insert into Book(BookName,AuthorID,[Read]) values(@BookName,@AuthorID,@Read)", 
               new { BookName = textBox1.Text, AuthorID = authorToAdd.AuthorID, Read = read});
        }
        private void FillGrid()
        {
            dataGridView1.Rows.Clear();
            //form kapanırken connectionı kapat
            List<Book> books = sqlConnection.Query<Book>(
                   "select * from Book").ToList();
            dataGridView1.ColumnCount = 5;

            dataGridView1.Columns[0].Name = "Read";
            dataGridView1.Columns[1].Name = "BookName";
            dataGridView1.Columns[2].Name = "AuthorName";

           
            foreach (var book in books)
            {
                if (book.Read == true) dataGridView1.Rows.Add("Yes", book.BookName, GetAuthorByID(book.AuthorID).AuthorName);
                else dataGridView1.Rows.Add("No", book.BookName, GetAuthorByID(book.AuthorID).AuthorName);
            }
        }
        public Form1()
        {
            InitializeComponent();
            FillGrid();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddBook(textBox2.Text, checkBox1.Checked);
            FillGrid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeleteRecord();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
           
        }

        private void textBox3_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
               textBox3.Text = Path.GetFileName(filePath);
                // Proceed with saving the path to SQL
            }
            else
            {
                // Handle cancel or error cases
            }
        }
    }
}
