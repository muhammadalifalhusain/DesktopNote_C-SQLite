
using DesktopNote.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace DesktopNote.Data;

public class NoteDatabase
{
    private readonly string _connectionString;

    public NoteDatabase()
    {
        string folderPath =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "DesktopNote");

        Directory.CreateDirectory(folderPath);

        string databasePath =
            Path.Combine(
                folderPath,
                "desktopnote.db");

        _connectionString =
            $"Data Source={databasePath}";

        InitializeDatabase();
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(
            _connectionString);
    }

    private void InitializeDatabase()
    {
        using var connection =
            CreateConnection();

        connection.Open();

        using var command =
            connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Notes
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Content TEXT NOT NULL,
                Type INTEGER NOT NULL,
                Color INTEGER NOT NULL,
                IsPinned INTEGER NOT NULL DEFAULT 0,
                IsArchived INTEGER NOT NULL DEFAULT 0,
                IsCompleted INTEGER NOT NULL DEFAULT 0,
                ReminderAt TEXT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL
            );
            """;

        command.ExecuteNonQuery();
    }

    public List<Note> GetNotes()
    {
        var notes =
            new List<Note>();

        using var connection =
            CreateConnection();

        connection.Open();

        using var command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                Id,
                Title,
                Content,
                Type,
                Color,
                IsPinned,
                IsArchived,
                IsCompleted,
                ReminderAt,
                CreatedAt,
                UpdatedAt
            FROM Notes
            ORDER BY IsPinned DESC, UpdatedAt DESC;
            """;

        using var reader =
            command.ExecuteReader();

        while (reader.Read())
        {
            notes.Add(
                ReadNote(reader));
        }

        return notes;
    }

    public Note InsertNote(
        Note note)
    {
        using var connection =
            CreateConnection();

        connection.Open();

        using var command =
            connection.CreateCommand();

        command.CommandText =
            """
            INSERT INTO Notes
            (
                Title,
                Content,
                Type,
                Color,
                IsPinned,
                IsArchived,
                IsCompleted,
                ReminderAt,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                $title,
                $content,
                $type,
                $color,
                $isPinned,
                $isArchived,
                $isCompleted,
                $reminderAt,
                $createdAt,
                $updatedAt
            );

            SELECT last_insert_rowid();
            """;

        command.Parameters.AddWithValue(
            "$title",
            note.Title);

        command.Parameters.AddWithValue(
            "$content",
            note.Content);

        command.Parameters.AddWithValue(
            "$type",
            (int)note.Type);

        command.Parameters.AddWithValue(
            "$color",
            (int)note.Color);

        command.Parameters.AddWithValue(
            "$isPinned",
            note.IsPinned ? 1 : 0);

        command.Parameters.AddWithValue(
            "$isArchived",
            note.IsArchived ? 1 : 0);

        command.Parameters.AddWithValue(
            "$isCompleted",
            note.IsCompleted ? 1 : 0);

        command.Parameters.AddWithValue(
            "$reminderAt",
            note.ReminderAt?.ToString("O")
                ?? (object)DBNull.Value);

        command.Parameters.AddWithValue(
            "$createdAt",
            note.CreatedAt.ToString("O"));

        command.Parameters.AddWithValue(
            "$updatedAt",
            note.UpdatedAt.ToString("O"));

        object? result =
            command.ExecuteScalar();

        note.Id =
            Convert.ToInt32(result);

        return note;
    }

    public void UpdateNote(
        Note note)
    {
        using var connection =
            CreateConnection();

        connection.Open();

        using var command =
            connection.CreateCommand();

        command.CommandText =
            """
            UPDATE Notes
            SET
                Title = $title,
                Content = $content,
                Type = $type,
                Color = $color,
                IsPinned = $isPinned,
                IsArchived = $isArchived,
                IsCompleted = $isCompleted,
                ReminderAt = $reminderAt,
                CreatedAt = $createdAt,
                UpdatedAt = $updatedAt
            WHERE Id = $id;
            """;

        command.Parameters.AddWithValue(
            "$id",
            note.Id);

        command.Parameters.AddWithValue(
            "$title",
            note.Title);

        command.Parameters.AddWithValue(
            "$content",
            note.Content);

        command.Parameters.AddWithValue(
            "$type",
            (int)note.Type);

        command.Parameters.AddWithValue(
            "$color",
            (int)note.Color);

        command.Parameters.AddWithValue(
            "$isPinned",
            note.IsPinned ? 1 : 0);

        command.Parameters.AddWithValue(
            "$isArchived",
            note.IsArchived ? 1 : 0);

        command.Parameters.AddWithValue(
            "$isCompleted",
            note.IsCompleted ? 1 : 0);

        command.Parameters.AddWithValue(
            "$reminderAt",
            note.ReminderAt?.ToString("O")
                ?? (object)DBNull.Value);

        command.Parameters.AddWithValue(
            "$createdAt",
            note.CreatedAt.ToString("O"));

        command.Parameters.AddWithValue(
            "$updatedAt",
            note.UpdatedAt.ToString("O"));

        command.ExecuteNonQuery();
    }

    public void DeleteNote(
        int id)
    {
        using var connection =
            CreateConnection();

        connection.Open();

        using var command =
            connection.CreateCommand();

        command.CommandText =
            """
            DELETE FROM Notes
            WHERE Id = $id;
            """;

        command.Parameters.AddWithValue(
            "$id",
            id);

        command.ExecuteNonQuery();
    }

    private static Note ReadNote(
        SqliteDataReader reader)
    {
        return new Note
        {
            Id =
                reader.GetInt32(0),

            Title =
                reader.GetString(1),

            Content =
                reader.GetString(2),

            Type =
                (NoteType)reader.GetInt32(3),

            Color =
                (NoteColor)reader.GetInt32(4),

            IsPinned =
                reader.GetInt32(5) == 1,

            IsArchived =
                reader.GetInt32(6) == 1,

            IsCompleted =
                reader.GetInt32(7) == 1,

            ReminderAt =
                reader.IsDBNull(8)
                    ? null
                    : DateTime.Parse(
                        reader.GetString(8)),

            CreatedAt =
                DateTime.Parse(
                    reader.GetString(9)),

            UpdatedAt =
                DateTime.Parse(
                    reader.GetString(10))
        };
    }
}