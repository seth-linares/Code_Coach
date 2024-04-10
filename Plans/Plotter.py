#%%
import matplotlib.pyplot as plt
import networkx as nx

# Define the graph
G = nx.DiGraph()

# Add nodes with the page name as the node ID
pages = [
    "Home Page", "Login/Registration Page", "Profile Page",
    "Coding Problems Index Page", "Coding Problem Detail Page",
    "2FA Setup Page", "Problem Attempt History Page", "Detailed Attempt View Page",
    "FAQs or Help Page", "Settings/Account Management Page", "API Key Management Page",
    "Leaderboard Page (Optional)", "Resource and Tutorial Page"
]

# Adding nodes to the graph
G.add_nodes_from(pages)

# Define edges (navigation) between pages
edges = [
    ("Home Page", "Login/Registration Page"),
    ("Login/Registration Page", "Home Page"),
    ("Login/Registration Page", "2FA Setup Page"),
    ("Login/Registration Page", "Profile Page"),
    ("Profile Page", "Settings/Account Management Page"),
    ("Profile Page", "API Key Management Page"),
    ("Profile Page", "Problem Attempt History Page"),
    ("Coding Problems Index Page", "Coding Problem Detail Page"),
    ("Coding Problem Detail Page", "Detailed Attempt View Page"),
    ("Detailed Attempt View Page", "Coding Problem Detail Page"),
    ("Detailed Attempt View Page", "Problem Attempt History Page"),
    ("Problem Attempt History Page", "Detailed Attempt View Page"),
    ("Settings/Account Management Page", "Profile Page"),
    ("API Key Management Page", "Profile Page"),
    ("Home Page", "FAQs or Help Page"),
    ("Home Page", "Coding Problems Index Page"),
    ("Home Page", "Resource and Tutorial Page"),
]

# Adding edges to the graph
G.add_edges_from(edges)

# Draw the graph
plt.figure(figsize=(30, 20))
pos = nx.spring_layout(G, k=0.15, iterations=20)
nx.draw(G, pos, with_labels=True, node_size=5000, node_color="skyblue", font_size=10, font_weight="bold", arrows=True)
plt.title("Site Page Navigation Map")
plt.axis("off")  # Turn off the axis
plt.show()
#%%