# RandomSelection
The inspiration for this library came from watching people who were entered into supposed random prize drawings win multiple times while others were consistently not picked.  RandomSelection is designed to take in entries, with the option to give some entries a higher chance at being selected, and then return one or more selectees.  It utilizes *System.Security.Cryptography* to ensure a higher quality random number is used for shuffling the data and selecting winners.  

**Examples**
1.  Create an instance of the Selector class, add two items representing employees and then selects one employee at random.

```c#
var selector = new Selector<string>();
selector.TryAddItem("m0392", "Doe, John", 1); 
selector.TryAddItem("m0392", "Moe, Jane", 1); 
var winner = selector.RandomSelect();
```
2.  Create an instance of the Selector class, add five items where the third entry has two more entries than the others.  Then two winners are selected
```c#
var selector = new Selector<string>();
selector.TryAddItem("jen", "jen", 1); 
selector.TryAddItem("michael", "michael", 1); 
selector.TryAddItem("staci", "staci", 3); 
selector.TryAddItem("dave", "dave", 1); 
selector.TryAddItem("leslie", "leslie", 1); 

var winners = selector.RandomSelect(2);
