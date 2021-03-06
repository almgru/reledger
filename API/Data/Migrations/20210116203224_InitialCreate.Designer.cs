﻿// <auto-generated />
using System;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20210116203224_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("API.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AccountId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("API.Entities.Attachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Data")
                        .HasColumnType("BLOB");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TransactionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("API.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TransactionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("API.Entities.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<int?>("CreditAccountId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Currency")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int?>("DebitAccountId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CreditAccountId");

                    b.HasIndex("DebitAccountId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("API.Entities.Account", b =>
                {
                    b.HasOne("API.Entities.Account", null)
                        .WithMany("Descendants")
                        .HasForeignKey("AccountId");
                });

            modelBuilder.Entity("API.Entities.Attachment", b =>
                {
                    b.HasOne("API.Entities.Transaction", null)
                        .WithMany("Attachments")
                        .HasForeignKey("TransactionId");
                });

            modelBuilder.Entity("API.Entities.Tag", b =>
                {
                    b.HasOne("API.Entities.Transaction", null)
                        .WithMany("Tags")
                        .HasForeignKey("TransactionId");
                });

            modelBuilder.Entity("API.Entities.Transaction", b =>
                {
                    b.HasOne("API.Entities.Account", "CreditAccount")
                        .WithMany()
                        .HasForeignKey("CreditAccountId");

                    b.HasOne("API.Entities.Account", "DebitAccount")
                        .WithMany()
                        .HasForeignKey("DebitAccountId");

                    b.Navigation("CreditAccount");

                    b.Navigation("DebitAccount");
                });

            modelBuilder.Entity("API.Entities.Account", b =>
                {
                    b.Navigation("Descendants");
                });

            modelBuilder.Entity("API.Entities.Transaction", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
