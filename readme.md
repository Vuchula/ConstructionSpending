# U.S. Housing Data Website

## Purpose
Shelter is a fundamental need for people. Reasearch has shown that housing is highly correlated to the health, economic and functional outcomes of it's residents. Despite the advancements we have seen in economic development and technological innovation, providing a residential shelter for people is still one of the largest shortcomings for most nations in the world.

While this information is freely available for anyone to see through open-source libraries, the data is often presented in different time and value formats that make trend-analysis not easy to digest. The purpose of this site is to provide interested parties with nation-wide quarterly data showing spending in residential construction versus the total inventory. This information can be digested at a macro-level thorough an interactive chart displayed the 'Visualize' tab, or easily dragged into comparison at a yearly level by selecting years of interest for comparison in the 'Years' tab.

## Distributed Information Components

Written in C# this project makes usage of API data drawn from the U.S. Census which is cleaned, transformed and stored in a SQL Server database. This data is then distributed in charts and comparison tables that can be filtered for an easy digestion by the user. 

Some of the techonologies used in building this site:
- Microsoft SQL Server Database
- MVC Framework
- Entity Frameworks: Code First 
- LINQ Queries
- Plotly Chart
- Azure App Service Deploymnet
