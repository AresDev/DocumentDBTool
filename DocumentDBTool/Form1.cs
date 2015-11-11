using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocumentDBTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Consultar_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != string.Empty)
            {
                textBox2.Text = GetDocument(textBox1.Text.Trim()).ToString();
            }
        }

        private Document GetDocument(string documentId)
        {
            Uri docURI = UriFactory.CreateDocumentUri(DBManager.Database.ToString(), DBManager.Collection.ToString(), textBox1.Text);
            Uri collectionURI = UriFactory.CreateDocumentCollectionUri(DocumentDBTool.Properties.Settings.Default.Database, DocumentDBTool.Properties.Settings.Default.Collection);
            var document = DBManager.Client.CreateDocumentQuery<Document>(collectionURI)
                .Where(doc => doc.Id == documentId)
                .AsEnumerable()
                .Single();
            return document;
        }

        private void Actualizar_Click(object sender, EventArgs e)
        {
            Uri collectionURI = UriFactory.CreateDocumentCollectionUri(DocumentDBTool.Properties.Settings.Default.Database, DocumentDBTool.Properties.Settings.Default.Collection);
            var result = DBManager.Client.UpsertDocumentAsync(collectionURI, JsonConvert.DeserializeObject(textBox2.Text)).Result;
            if (result != null)
            {
                textBox2.Clear();
                textBox2.Text = GetDocument(textBox1.Text.Trim()).ToString();
                MessageBox.Show("Actualizado correctamente");
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error, por favor intente nuevamente");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }
    }
}
