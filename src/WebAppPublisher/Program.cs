using WebAppPublisher;
using WebAppPublisher.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.UseStartup<Startup>();

