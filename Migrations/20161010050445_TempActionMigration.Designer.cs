﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Hourai;

namespace Hourai.Migrations
{
    [DbContext(typeof(BotDbContext))]
    [Migration("20161010050445_TempActionMigration")]
    partial class TempActionMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("Hourai.AbstractTempAction", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<DateTimeOffset>("End");

                    b.Property<ulong>("GuildId");

                    b.Property<DateTimeOffset>("Start");

                    b.Property<ulong>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("End")
                        .HasName("IX_temp_actions_End");

                    b.ToTable("temp_actions");

                    b.HasDiscriminator<string>("Discriminator").HasValue("AbstractTempAction");
                });

            modelBuilder.Entity("Hourai.Channel", b =>
                {
                    b.Property<ulong>("Id");

                    b.Property<ulong>("GuildId");

                    b.Property<bool>("BanMessage");

                    b.Property<bool>("JoinMessage");

                    b.Property<bool>("LeaveMessage");

                    b.Property<bool>("SearchIgnored");

                    b.HasKey("Id", "GuildId");

                    b.HasIndex("GuildId");

                    b.ToTable("channels");
                });

            modelBuilder.Entity("Hourai.CustomCommand", b =>
                {
                    b.Property<ulong>("GuildId");

                    b.Property<string>("Name");

                    b.Property<string>("Response")
                        .IsRequired();

                    b.HasKey("GuildId", "Name");

                    b.HasIndex("GuildId");

                    b.ToTable("commands");
                });

            modelBuilder.Entity("Hourai.Guild", b =>
                {
                    b.Property<ulong>("Id");

                    b.Property<bool>("IsBlacklisted");

                    b.Property<string>("MinRoles");

                    b.Property<long>("Modules");

                    b.HasKey("Id");

                    b.ToTable("guilds");
                });

            modelBuilder.Entity("Hourai.GuildUser", b =>
                {
                    b.Property<ulong>("Id");

                    b.Property<ulong>("GuildId");

                    b.Property<string>("BannedRoles");

                    b.Property<ulong?>("UserId");

                    b.HasKey("Id", "GuildId");

                    b.HasIndex("GuildId");

                    b.HasIndex("UserId");

                    b.ToTable("guild_users");
                });

            modelBuilder.Entity("Hourai.User", b =>
                {
                    b.Property<ulong>("Id");

                    b.Property<bool>("IsBlacklisted");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("Hourai.Username", b =>
                {
                    b.Property<ulong>("UserId");

                    b.Property<DateTimeOffset>("Date");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("UserId", "Date");

                    b.HasIndex("UserId");

                    b.ToTable("usernames");
                });

            modelBuilder.Entity("Hourai.TempBan", b =>
                {
                    b.HasBaseType("Hourai.AbstractTempAction");


                    b.ToTable("temp_actions");

                    b.HasDiscriminator().HasValue("TempBan");
                });

            modelBuilder.Entity("Hourai.TempRole", b =>
                {
                    b.HasBaseType("Hourai.AbstractTempAction");

                    b.Property<ulong>("RoleId");

                    b.ToTable("temp_actions");

                    b.HasDiscriminator().HasValue("TempRole");
                });

            modelBuilder.Entity("Hourai.Channel", b =>
                {
                    b.HasOne("Hourai.Guild", "Guild")
                        .WithMany("Channels")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Hourai.CustomCommand", b =>
                {
                    b.HasOne("Hourai.Guild", "Guild")
                        .WithMany("Commands")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Hourai.GuildUser", b =>
                {
                    b.HasOne("Hourai.Guild", "Guild")
                        .WithMany("Users")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Hourai.User")
                        .WithMany("GuildUsers")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Hourai.Username", b =>
                {
                    b.HasOne("Hourai.User", "User")
                        .WithMany("Usernames")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
