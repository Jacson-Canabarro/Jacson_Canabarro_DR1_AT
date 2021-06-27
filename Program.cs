using Aplicacao.Servicos;
using Dominio.Entidades;
using Dominio.Interfaces.Aplicacao;
using Dominio.Interfaces.Infra;
using Infra.Query;
using Infra.Repositorios;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace gerenciador_fundamentos
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnectionSql"));
            UsuarioConsultas consultas = new UsuarioConsultas(connection);
            IUsuarioRepositorio repositorio = new UsuarioRepositorio(consultas);
            IUsuarioServico servico = new UsuarioServico(repositorio);

            Console.WriteLine("==============================================");
            Console.WriteLine("=Bem vindo ao gerenciador de aniversariantes==");
            Console.WriteLine("==============================================");

            var aniversariantes = await servico.GetUsuariosAniversariantes();
            if (aniversariantes != null)
            {
                Console.WriteLine("==============================================");
                Console.WriteLine("=========== Aniversariantes do dia ===========");
                Console.WriteLine("==============================================");
                Console.WriteLine(JsonConvert.SerializeObject(aniversariantes, Formatting.Indented));

            }
            else
            {
                Console.WriteLine("============== Não há aniversariantes hoje ===========");
            }
            var sair = false;

            while (!sair)
            {
                Console.WriteLine("==============================================");
                Console.WriteLine("===============Menu Iterativo =============== ");
                Console.WriteLine("==============================================");
                ExibeOpções();
                Console.WriteLine("===========Digite uma das opções acima =======");
                var opt = Console.ReadLine();

                switch (opt)
                {
                    case "1":
                        await CadastroUsuario();
                        break;
                    case "2":
                        await AtualizarUsuario();
                        break;
                    case "3":
                        await ExcluirUsuario();
                        break;
                    case "4":
                        await ObterUsuario();
                        break;
                    case "5":
                        await ListarTodosUsuarios();
                        break;
                    case "6":
                        await ObterUsuarioPorNome();
                        break;
                    case "7":
                        await ObterUsuariosAniversariantes();
                        break;
                    case "q":
                        Console.WriteLine("Até Logo :)");
                        sair = true;
                        break;
                    default:
                        Console.WriteLine("Opção Invalida!");
                        break;
                }


            }



            async Task ListarTodosUsuarios()
            {
                Console.WriteLine("Exibindo usuarios");
                Console.WriteLine("");
                var usuarios = await servico.GetAll();
                Console.WriteLine(JsonConvert.SerializeObject(usuarios, Formatting.Indented));
                Console.WriteLine("");
                Console.WriteLine("");
                await Task.CompletedTask;
            }

            async Task ObterUsuariosAniversariantes()
            {
                Console.WriteLine("Exibindo usuarios que fazem aniversario hoje");
                Console.WriteLine("");
                var usuarios = await servico.GetUsuariosAniversariantes();
                Console.WriteLine(JsonConvert.SerializeObject(usuarios, Formatting.Indented));
                Console.WriteLine("");
                Console.WriteLine("");
                await Task.CompletedTask;
            }

            async Task ObterUsuario()
            {
                Console.WriteLine("");
                Console.WriteLine("Digite o identificado do usuario");

                var id = Convert.ToInt32(Console.ReadLine());
                var usuario = await servico.GetById(id);

                Console.WriteLine("");
                Console.WriteLine($"Exibindo o usuario com o identificador {id}");
                Console.WriteLine("");
                Console.WriteLine(JsonConvert.SerializeObject(usuario, Formatting.Indented));
                Console.WriteLine("");
                Console.WriteLine("");
                await Task.CompletedTask;

            }

            async Task ObterUsuarioPorNome()
            {
                Console.WriteLine("");
                Console.WriteLine("Digite a palavra chave para buscar um usuario");

                var palavra = Console.ReadLine();
                var usuario = await servico.GetUsuarioByNomeOrSobreNome(palavra);

                Console.WriteLine("");
                Console.WriteLine($"Exibindo os usuarios encontrados");
                Console.WriteLine("");
                Console.WriteLine(JsonConvert.SerializeObject(usuario, Formatting.Indented));
                Console.WriteLine("");
                Console.WriteLine("");
                await Task.CompletedTask;

            }

            async Task ExcluirUsuario()
            {
                Console.WriteLine("");
                Console.WriteLine("Digite o identificador do usuario para realizar a exclusão");

                var id = Convert.ToInt32(Console.ReadLine());

                var usuario = await servico.GetById(id);

                if (usuario == null)
                    Console.WriteLine("Usuario não encontrado");
                else
                {
                    servico.Delete(usuario);
                    Console.WriteLine("Usuario excluido com sucesso");
                }
                await Task.CompletedTask;
            }

            async Task AtualizarUsuario()
            {
                Console.WriteLine("");
                Console.WriteLine("Digite o identificado do usuario");

                var id = Convert.ToInt32(Console.ReadLine());
                var usuario = await servico.GetById(id);
                Console.WriteLine("Informe o nome");
                usuario.Nome = Console.ReadLine();

                Console.WriteLine("Informe o sobrenome");
                usuario.SobreNome = Console.ReadLine();

                Console.WriteLine("Informe a data de aniversario (yyyy-MM-dd)");
                usuario.DataAniversario = Convert.ToDateTime(Console.ReadLine());

                servico.Update(usuario);

                Console.WriteLine("Usuario atualizado com sucesso!");
                await Task.CompletedTask;
            }

            async Task CadastroUsuario()
            {
                var usuario = new Usuario();

                Console.WriteLine("Informe o nome");
                usuario.Nome = Console.ReadLine();

                Console.WriteLine("Informe o sobrenome");
                usuario.SobreNome = Console.ReadLine();

                Console.WriteLine("Informe a data de aniversario (yyyy-MM-dd)");
                usuario.DataAniversario = Convert.ToDateTime(Console.ReadLine());

                await servico.Insert(usuario);

                Console.WriteLine("Usuario cadastrado com sucesso!");
                await Task.CompletedTask;
            }

            void ExibeOpções()
            {
                Console.WriteLine("Selecione as opções");
                Console.WriteLine("1 - Para criar um Usuario");
                Console.WriteLine("2 - Para atualizar um Usuario");
                Console.WriteLine("3 - Para excluir um Usuario");
                Console.WriteLine("4 - Para obter um Usuario pelo ID");
                Console.WriteLine("5 - Para exibir todos os Usuarios");
                Console.WriteLine("6 - Para buscar usuarios por uma palavra chave");
                Console.WriteLine("7 - Para buscar usuarios que fazem aniversario Hoje");
                Console.WriteLine("q - Para sair");
            }

            await Task.CompletedTask;
        }
    }
}
