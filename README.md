# Streaming Setup

## Entities / Context

- **Client**: Wants data at the moment of his request

- **Outer Service**: Can only return data in streams, piece by piece no instantly

- **InnerService**: Handles this expectation problem and creates a way that the **Client** can get his data in the momento of the request while recieving data from the **OuterSerice** 

## How?

1. **Client** request the **InnerService** to start collecting the data from **OuterService**
2. **InnerService** instantly responds with an Id relative to a Queue that will have the data that the **Client** desires
3. **Client** can request with this Id so it will recieve a piece of the data and a information if that was the last part of the whole
4. While the **Client** requests pieces of the data, the **InnerService** will be recieve parts of(streams) that and will store them in the Queue related to the Given Queue
