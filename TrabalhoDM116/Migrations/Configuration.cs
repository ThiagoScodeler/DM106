namespace TrabalhoDM116.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TrabalhoDM116.Models.TrabalhoDM116Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TrabalhoDM116.Models.TrabalhoDM116Context context)
        {
            context.Products.AddOrUpdate(
                p => p.Id,
                new Product { Id = 1, nome = "produto 1", descricao = "descrição produto 1", cor = "preto", modelo = "1P", codigo = "COD1", preco = 20, peso = 2, altura = 15, largura = 18, comprimento = 20, diametro = 20, url = "www.scodeler.com/produto1" },
                new Product { Id = 2, nome = "produto 2", descricao = "descrição produto 2", cor = "branco", modelo = "2P", codigo = "COD2", preco = 20, peso = 3, altura = 15, largura = 20, comprimento = 20, diametro = 16, url = "www.scodeler.com/produto2" },
                new Product { Id = 3, nome = "produto 3", descricao = "descrição produto 3", cor = "azul", modelo = "3P", codigo = "COD3", preco = 30, peso = 4, altura = 17, largura = 17, comprimento = 16, diametro = 22, url = "www.scodeler.com/produto3" },
                new Product { Id = 4, nome = "produto 4", descricao = "descrição produto 4", cor = "amarelo", modelo = "4P", codigo = "COD3", preco = 40, peso = 5, altura = 15, largura = 16, comprimento = 14, diametro = 21, url = "www.scodeler.com/produto4" },
                new Product { Id = 5, nome = "produto 5", descricao = "descrição produto 5", cor = "verde", modelo = "5P", codigo = "COD5", preco = 50, peso = 6, altura = 16, largura = 15, comprimento = 15, diametro = 17, url = "www.scodeler.com/produto5" },
                new Product { Id = 6, nome = "produto 6", descricao = "descrição produto 6", cor = "roxo", modelo = "6P", codigo = "COD6", preco = 60, peso = 7, altura = 17, largura = 18, comprimento = 14, diametro = 15, url = "www.scodeler.com/produto6" },
                new Product { Id = 7, nome = "produto 7", descricao = "descrição produto 7", cor = "marrom", modelo = "7P", codigo = "COD7", preco = 70, peso = 8, altura = 18, largura = 19, comprimento = 70, diametro = 26, url = "www.scodeler.com/produto7" },
                new Product { Id = 8, nome = "produto 8", descricao = "descrição produto 8", cor = "rosa", modelo = "8P", codigo = "COD8", preco = 80, peso = 9, altura = 19, largura = 20, comprimento = 19, diametro = 35, url = "www.scodeler.com/produto8" },
                new Product { Id = 9, nome = "produto 9", descricao = "descrição produto 9", cor = "laranja", modelo = "9P", codigo = "COD9", preco = 90, peso = 10, altura = 20, largura = 20, comprimento = 21, diametro = 22, url = "www.scodeler.com/produto9" }
            );
        }
    }
}
