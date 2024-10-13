using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SematicKernelApp.Config;
using SematicKernelApp.Extensions;

namespace SematicKernelApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IDictionary<string, string>? envVars;
        Kernel? kernel;
        ChatHistory history = [];
        IChatCompletionService chatCompletionService;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigExtensions.FromConfig<OpenAIConfig>("OneApiSpark");


            // Create kernel
            var builder = Kernel.CreateBuilder();

            var handler = new OpenAICustomHandler(config.Endpoint);

            builder.AddOpenAIChatCompletion(
                modelId: config.ModelId,
                apiKey: config.ApiKey,
                httpClient: new HttpClient(handler));

            var kernel = builder.Build();

            // Get chat completion service
            chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();


        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            loading1.Visibility = Visibility.Visible;
            string question = textBox1.Text;

            // 获取用户输入
            history.AddUserMessage(question);

            // 启用自动函数调用
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            // 获取人工智能的回应
            var result = await chatCompletionService.GetChatMessageContentAsync(
                history,
                executionSettings: openAIPromptExecutionSettings,
                kernel: kernel);

            // 打印结果        
            richTextBox2.AppendText(result.ToString());

            // 将客服人员的消息添加到聊天历史记录中
            history.AddMessage(result.Role, result.Content);

            loading1.Visibility = Visibility.Hidden;

        }
    }
}