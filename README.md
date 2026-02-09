# RandomSelection

A flexible .NET library for truly random selection with weighted entries. Perfect for raffles, lottery drawings, contests, and any scenario where you need fair, cryptographically-secure random selection.

## Overview

The inspiration for this library came from observing bias in prize drawings where participants won multiple times while others were consistently overlooked. **RandomSelection** is designed to ensure fair, unbiased selection by:

- **Accepting weighted entries** - Give some participants higher chances than others without complex probability calculations
- **Using cryptographic randomness** - Utilizes `System.Security.Cryptography` for high-quality random numbers instead of weak pseudo-random generators
- **Shuffling the selection pool** - Randomizes the entire pool before selection to eliminate positional bias
- **Supporting multiple selections** - Select one or many winners in a single operation
- **Type-safe and flexible** - Generic `Selector<T>` works with any data type

## Installation

Install via NuGet:

```bash
dotnet add package RandomSelection
```

Or via Package Manager Console:

```
Install-Package RandomSelection
```

## Core Concepts

### The Selector<T> Class

The `Selector<T>` class is the main entry point. It's generic and can work with any type of value you want to track. It maintains a pool of items and provides methods to randomly select from them.

```csharp
// Works with strings
var stringSelector = new Selector<string>();

// Works with custom objects
var objectSelector = new Selector<Employee>();

// Works with any type
var intSelector = new Selector<int>();
```

### Entries (Weights)

Each item can have multiple "entries" in the selection pool. This creates a weighted random selection:
- An item with **1 entry** has a baseline chance
- An item with **3 entries** is **3x more likely** to be selected
- An item with **10 entries** is **10x more likely** to be selected

**Real-world example:** In a loyalty rewards drawing where VIP members should win 4x more often:
```
- Regular member: 1 entry
- Silver member: 2 entries  
- Gold member: 3 entries
- Platinum member: 4 entries
```

## Quick Start

### Example 1: Simple Random Selection

Select one item from a group:

```csharp
using BNolan.RandomSelection;

var selector = new Selector<string>();
selector.TryAddItem("emp001", "John Doe");
selector.TryAddItem("emp002", "Jane Smith");
selector.TryAddItem("emp003", "Mike Johnson");

// Select 1 random employee
var winners = selector.RandomSelect(1);
Console.WriteLine($"Winner: {winners[0].Value}");
// Output: Winner: Mike Johnson (example)
```

### Example 2: Weighted Random Selection

Give some participants higher chances of winning:

```csharp
var selector = new Selector<string>();

// Regular members: 1 entry each
selector.TryAddItem("jen", "Jennifer", 1);
selector.TryAddItem("michael", "Michael", 1);
selector.TryAddItem("dave", "David", 1);

// VIP member: 3 entries (3x more likely to win)
selector.TryAddItem("staci", "Staci", 3);

// Select 2 winners from the pool
var winners = selector.RandomSelect(2);

foreach (var winner in winners)
{
    Console.WriteLine($"Winner: {winner.Value}");
}
```

### Example 3: Using Custom Objects

Work with any type of data, not just strings:

```csharp
using BNolan.RandomSelection;
using BNolan.RandomSelection.Library;

public class Employee
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public decimal Salary { get; set; }
}

var selector = new Selector<Employee>();

var alice = new Employee { Id = "001", Name = "Alice", Department = "Engineering", Salary = 120000 };
var bob = new Employee { Id = "002", Name = "Bob", Department = "Sales", Salary = 90000 };

selector.TryAddItem("emp001", alice);
selector.TryAddItem("emp002", bob);

var winner = selector.RandomSelect(1);
Console.WriteLine($"Selected: {winner[0].Value.Name} from {winner[0].Value.Department}");
```

### Example 4: Using the Item<T> Class

For explicit control over all properties, use the `Item<T>` class:

```csharp
using BNolan.RandomSelection.Library;

var selector = new Selector<string>();

var item1 = new Item<string>
{
    UniqueId = "ticket001",
    Value = "John Doe",
    Entries = 2
};

var item2 = new Item<string>
{
    UniqueId = "ticket002",
    Value = "Jane Smith",
    Entries = 1
};

selector.TryAddItem(item1);
selector.TryAddItem(item2);

var winner = selector.RandomSelect();
Console.WriteLine($"Winner: {winner[0].Value}");
```

### Example 5: Bulk Operations

Add multiple items efficiently in one operation:

```csharp
var employees = new List<string> { "Alice", "Bob", "Charlie", "Diana" };

var selector = new Selector<string>();
if (selector.TryAddItems(employees))
{
    Console.WriteLine("All employees added successfully");
    var selected = selector.RandomSelect(2);
}
else
{
    Console.WriteLine("Failed to add employees");
}
```

## API Reference

### Adding Items

#### `bool TryAddItem(T uniqueId)`

Adds an item with just a unique ID. The value defaults to the uniqueId, and entries default to 1.

```csharp
bool success = selector.TryAddItem("participant1");
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `uniqueId` | T | Unique identifier (case-insensitive) |
| **Returns** | bool | `true` if added, `false` if ID already exists |

---

#### `bool TryAddItem(string uniqueId, T value)`

Adds an item with a unique ID and associated value. Entries default to 1.

```csharp
bool success = selector.TryAddItem("emp001", "John Doe");
bool success = selector.TryAddItem("ticket123", myCustomObject);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `uniqueId` | string | Unique identifier (case-insensitive) |
| `value` | T | Associated value of any type |
| **Returns** | bool | `true` if added, `false` if ID already exists |

---

#### `bool TryAddItem(string uniqueId, T value, int entries)`

Adds an item with a unique ID, value, and number of entries (weight).

```csharp
// Regular member: 1 entry
selector.TryAddItem("user001", "Regular User", 1);

// VIP member: 5x more likely to be selected
selector.TryAddItem("user002", "VIP Member", 5);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `uniqueId` | string | Unique identifier (case-insensitive) |
| `value` | T | Associated value of any type |
| `entries` | int | Number of entries/weight (must be > 0) |
| **Returns** | bool | `true` if added, `false` if ID already exists |
| **Throws** | ArgumentException | If entries < 1 |

---

#### `bool TryAddItem(Item<T> item)`

Adds an `Item<T>` object directly for maximum control.

```csharp
var item = new Item<string> 
{ 
    UniqueId = "id1", 
    Value = "My Value", 
    Entries = 3 
};

if (selector.TryAddItem(item))
{
    Console.WriteLine("Item added successfully");
}
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `item` | Item<T> | Item object with UniqueId, Value, and Entries |
| **Returns** | bool | `true` if added, `false` if ID already exists |
| **Throws** | ArgumentNullException | If item is null or UniqueId is null/empty |
| **Throws** | ArgumentException | If Entries < 1 |

---

#### `bool TryAddItems(List<T> items)`

Adds multiple items at once. This is an all-or-nothing operation: if any item already exists, no items are added.

```csharp
var employees = new List<string> { "Alice", "Bob", "Charlie" };

if (selector.TryAddItems(employees))
{
    Console.WriteLine("All employees added");
}
else
{
    Console.WriteLine("Failed: at least one employee already exists");
}
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `items` | List<T> | Items to add |
| **Returns** | bool | `true` if all items added, `false` if any already exist |
| **Throws** | ArgumentNullException | If items list is null |

---

### Selection Methods

#### `List<Item<T>> RandomSelect(int numToSelect = 1)`

Randomly selects items from the pool. Each item can only be selected once per call.

```csharp
// Select 1 winner (default)
var winner = selector.RandomSelect();

// Select 3 winners
var topThree = selector.RandomSelect(3);

foreach (var item in topThree)
{
    Console.WriteLine($"ID: {item.UniqueId}, Value: {item.Value}");
}
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `numToSelect` | int | Number of items to select (default: 1) |
| **Returns** | List<Item<T>> | List of selected Item objects |

**Return Structure:**
```csharp
List<Item<T>>
{
    new Item<T> { UniqueId = "id1", Value = yourValue, Entries = 1 },
    new Item<T> { UniqueId = "id2", Value = yourValue, Entries = 3 }
}
```

---

### Utility Methods

#### `List<string> GenerateList()`

Creates the internal pool list with items repeated according to their entry count. Useful for debugging or understanding how the pool is constructed.

```csharp
selector.TryAddItem("id1", "value1", 2);
selector.TryAddItem("id2", "value2", 1);

var pool = selector.GenerateList();
// Result: ["id1", "id1", "id2"]

Console.WriteLine($"Pool size: {pool.Count}");
```

| **Returns** | List<string> | List of unique IDs, each repeated by its entry count |

---

#### `List<string> RandomizeList(List<string> items)`

Shuffles a list using cryptographic randomness. Can be used independently of selection.

```csharp
var original = new List<string> { "a", "b", "c", "d", "e" };
var shuffled = selector.RandomizeList(original);

// original remains unchanged
// shuffled contains a random permutation of the items
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `items` | List<string> | List to shuffle |
| **Returns** | List<string> | New shuffled list (original is not modified) |

---

#### `int GenerateRandomIndex(int upperLimit)`

Generates a random index between 0 and upperLimit (exclusive). Uses cryptographic randomness and can be used independently.

```csharp
int randomIndex = selector.GenerateRandomIndex(100);
// Returns a random number from 0 to 99

int diceRoll = selector.GenerateRandomIndex(6);
// Simulates a die roll (0-5)
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `upperLimit` | int | Upper bound (exclusive) |
| **Returns** | int | Random integer from 0 to upperLimit - 1 |
| **Throws** | ArgumentOutOfRangeException | If upperLimit < 1 |

---

## Error Handling

All add methods validate input and throw appropriate exceptions. Always handle these when accepting user input:

```csharp
try
{
    selector.TryAddItem((Item<string>)null);  // ArgumentNullException
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"Validation error: {ex.ParamName}");
}

try
{
    selector.TryAddItem("id1", "value", 0);  // ArgumentException
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid argument: {ex.ParamName}");
}
```

**Common Exceptions:**
- `ArgumentNullException` - When item or UniqueId is null/empty
- `ArgumentException` - When entries < 1
- `ArgumentOutOfRangeException` - When GenerateRandomIndex called with upperLimit < 1

---

## Why Cryptographic Randomness?

This library uses `System.Security.Cryptography` instead of `System.Random` for several important reasons:

| Feature | System.Random | Cryptographic RNG |
|---------|---------------|-------------------|
| **Predictable** | Yes (if seed known) | No - truly random |
| **Suitable for fairness** | No | Yes ? |
| **Passes statistical tests** | No | Yes ? |
| **Seed bias** | Yes | No ? |
| **Reproducible** | Yes (sometimes a problem) | No |

For raffles and contests where fairness matters, cryptographic randomness is essential.

---

## Performance Considerations

- **First selection**: Building the pool is O(n) where n = total number of entries. This happens once per `RandomSelect()` call.
- **Shuffling**: Uses Fisher-Yates algorithm, O(n) complexity
- **Cryptographic calls**: Each random index requires at least one cryptographic RNG call
- **Repeated selections**: Reuse the same Selector instance for multiple selections to avoid rebuilding the pool

**Optimization Tip:** For large selections, it's more efficient to call `RandomSelect(10)` once rather than `RandomSelect(1)` ten times.

---

## Common Use Cases

### Raffle/Lottery Drawing

```csharp
var selector = new Selector<string>();

// Add raffle participants
selector.TryAddItem("ticket001", "John Smith");
selector.TryAddItem("ticket002", "Jane Doe");
selector.TryAddItem("ticket003", "Bob Johnson");
selector.TryAddItem("ticket004", "Alice Williams");

// Draw 3 winners
var winners = selector.RandomSelect(3);

foreach (var winner in winners)
{
    Console.WriteLine($"Winner: {winner.Value}");
}
```

### Contest with VIP Privileges

```csharp
var selector = new Selector<string>();

// Regular participants (1x chance)
selector.TryAddItem("regular001", "Regular User 1", 1);
selector.TryAddItem("regular002", "Regular User 2", 1);

// VIP gets enhanced odds
selector.TryAddItem("vip001", "Premium User", 5);

var winner = selector.RandomSelect(1);
Console.WriteLine($"Contest winner: {winner[0].Value}");
```

### Team Assignment Rotation

```csharp
var employees = new List<(string id, string name, string role)>
{
    ("emp001", "Alice Johnson", "Developer"),
    ("emp002", "Bob Smith", "Designer"),
    ("emp003", "Charlie Brown", "QA"),
    ("emp004", "Diana Prince", "Manager")
};

var selector = new Selector<(string, string)>();
foreach (var emp in employees)
{
    selector.TryAddItem(emp.id, (emp.name, emp.role));
}

var assignedEmployee = selector.RandomSelect(1)[0];
Console.WriteLine($"On-call: {assignedEmployee.Value.Item1} ({assignedEmployee.Value.Item2})");
```

### Lottery with Weighted Chances

```csharp
public class LotteryPlayer
{
    public string Name { get; set; }
    public int TicketsPurchased { get; set; }
}

var selector = new Selector<LotteryPlayer>();

selector.TryAddItem("player001", new LotteryPlayer { Name = "Alice", TicketsPurchased = 5 }, 5);
selector.TryAddItem("player002", new LotteryPlayer { Name = "Bob", TicketsPurchased = 3 }, 3);
selector.TryAddItem("player003", new LotteryPlayer { Name = "Charlie", TicketsPurchased = 10 }, 10);

// Winners weighted by ticket count
var winner = selector.RandomSelect(1);
Console.WriteLine($"Jackpot winner: {winner[0].Value.Name}");
```

---

## Features at a Glance

? **Weighted selection** - Control probability with entry counts  
? **Cryptographically secure** - Uses `System.Security.Cryptography`  
? **Generic support** - Works with any data type (T)  
? **Bulk operations** - Add and select multiple items efficiently  
? **No duplicates** - Each selection returns unique winners  
? **Comprehensive validation** - Clear error messages  
? **Thoroughly tested** - Includes statistical randomness tests  
? **Easy API** - Intuitive methods with sensible defaults  

---

## Testing

The library includes comprehensive unit tests covering:
- Basic functionality (adding items, selection)
- Error handling and validation
- Statistical randomness verification
- Edge cases (single item, all items, duplicates)
- Distribution uniformity tests

Run tests with:
```bash
dotnet test
```

---

## License

MIT License - See repository for details
