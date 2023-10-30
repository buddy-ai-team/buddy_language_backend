﻿// <auto-generated />
using System;
using BuddyLanguage.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BuddyLanguage.Infrastructure.Migrations
{
    [DbContext(typeof(ChatGptDbContextTmp))]
    partial class ChatGptDbContextTmpModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("OpenAI.ChatGpt.Models.PersistentChatMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasAnnotation("Relational:JsonPropertyName", "content");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasAnnotation("Relational:JsonPropertyName", "role");

                    b.Property<Guid>("TopicId")
                        .HasColumnType("char(36)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("OpenAI.ChatGpt.Models.Topic", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("OpenAI.ChatGpt.Models.Topic", b =>
                {
                    b.OwnsOne("OpenAI.ChatGpt.Models.ChatGPTConfig", "Config", b1 =>
                        {
                            b1.Property<Guid>("TopicId")
                                .HasColumnType("char(36)");

                            b1.Property<string>("InitialSystemMessage")
                                .HasColumnType("longtext");

                            b1.Property<string>("InitialUserMessage")
                                .HasColumnType("longtext");

                            b1.Property<int?>("MaxTokens")
                                .HasColumnType("int");

                            b1.Property<string>("Model")
                                .HasColumnType("longtext");

                            b1.Property<bool?>("PassUserIdToOpenAiRequests")
                                .HasColumnType("tinyint(1)");

                            b1.Property<float?>("Temperature")
                                .HasColumnType("float");

                            b1.HasKey("TopicId");

                            b1.ToTable("Topics");

                            b1.WithOwner()
                                .HasForeignKey("TopicId");
                        });

                    b.Navigation("Config")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
