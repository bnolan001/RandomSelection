# RandomSelection
The inspiration for this library came from watching people who were entered into supposed random prize drawings win multiple times while others consistently are not picked.  RandomSelection is designed to take in entries, with the option to give some entries a higher chance at being selected, and then return one ore more selectees.  It utilizes *System.Security.Cryptography* to ensure a higher quality random number is used for shuffling the data and selecting winners.  

The following code creates an instance of the Selector class, adds two items representing employees and then selects one employee at random.

```c#
Selector selector = new Selector();
selector.TryAddItem("m0392", "Doe, John", 1); 
selector.TryAddItem("m0392", "Moe, Jane", 1); 
selector.RandomSelect();
```
