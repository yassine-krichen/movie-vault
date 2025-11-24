# Database Seeding Guide

## Overview

The application includes a comprehensive database seeding system that allows you to populate the database with sample data. The seeder includes:

-   **5 membership plans** with varying fees, durations, and discount rates
-   **10 genres** (Action, Comedy, Drama, Horror, Romance, Sci-Fi, Thriller, Animation, Documentary, Fantasy)
-   **50 movies** with dynamic random dates (within the last year) and default image assignment
-   **20 customers** with random memberships and movie associations

## How to Use

### Option 1: Seed Database (Without Reset)

If the database is empty, this will add sample data. If data already exists, it will skip:

```bash
dotnet run -- --seed
```

### Option 2: Seed Database with Reset

This will reset the database first, then add fresh sample data. Useful for starting over:

```bash
dotnet run -- --seed-reset
```

### Option 3: Force Seed (Ignore Existing Data)

If the database has data but you want to add more anyway, use the `--force` flag with `--seed`:

```bash
dotnet run -- --seed --force
```

### Option 4: Normal Run (No Seeding)

Just run the application without any seeding flags:

```bash
dotnet run
```

## Command Details

| Command                        | Behavior                                      |
| ------------------------------ | --------------------------------------------- |
| `dotnet run`                   | Runs the application normally without seeding |
| `dotnet run -- --seed`         | Checks if data exists; only seeds if empty    |
| `dotnet run -- --seed-reset`   | Resets database, then seeds with fresh data   |
| `dotnet run -- --seed --force` | Adds sample data even if data already exists  |

## Data Characteristics

### Memberships

-   **Free Trial**: $0 signup, 1 month, 0% discount
-   **Basic**: $9.99 signup, 3 months, 5% discount
-   **Standard**: $19.99 signup, 6 months, 10% discount
-   **Premium**: $29.99 signup, 12 months, 15% discount
-   **Ultimate**: $49.99 signup, 24 months, 20% discount

### Movies

-   50 randomly-selected movie titles across all genres
-   Each movie assigned to a random genre
-   Creation dates randomized within the last 365 days
-   Default image: `/images/movies/default.png`

### Customers

-   20 randomly-generated customers with realistic first/last names
-   Each assigned to a random membership plan
-   Each associated with 0-8 random movies
-   Ensures diverse data for testing

## Sample Output

When you run with `--seed-reset`, you'll see:

```
Resetting database before seeding...
Database reset successfully
Starting database seeding...
Seeded 5 memberships
Seeded 10 genres
Seeded 50 movies
Seeded 20 customers
Database seeding completed successfully
```

## Features

✅ **Smart Duplicate Prevention**: Checks if data exists before seeding  
✅ **Reset Option**: Optional database reset before seeding  
✅ **Force Option**: Override existing data if needed  
✅ **Dynamic Data**: Different movie/customer assignments each run  
✅ **Proper Foreign Keys**: All relationships properly maintained  
✅ **Default Images**: Uses `/images/movies/default.png` for all seeded movies  
✅ **Logging**: Comprehensive logging for debugging  
✅ **Error Handling**: Graceful error handling with detailed error messages

## Notes

-   Seeding runs on application startup if a seeding flag is provided
-   After seeding completes, the application exits (returns control to terminal)
-   The seeder only creates data; it doesn't interfere with normal application operation
-   All timestamps are set to `DateTime.Now` (server local time)
-   Movie-Customer associations are random but valid (no duplicate associations per customer)
