Notes:
sln file located in Synapse.OrderProcessing.Service folder
Work not completed:
Did not write unit tests for Orders and OrderProcessing services
Those should be completed before "Production Ready"
Handling params via settings file this would be better handled with env variables or Azure App Configuration in production
Since original code was a console app I left as such and configured to run process every 5 minutes. This may not be the best solution for deployment
and probably better to deploy as an Azure app function. Tried to build in a such a way as to make refactoring easy for a different deployment strategy.
