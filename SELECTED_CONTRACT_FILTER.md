# Selected Contract Filter Feature

## Overview
The selected contract filter is a global UI feature that allows users to filter data across multiple pages to show only items associated with a specific contract.

## How It Works

### Frontend Implementation

#### ContractSelection Context
- Location: `frontend/src/contexts/ContractSelectionContext.tsx`
- Provides global state for the selected contract
- Syncs the selected contract with the URL query parameter `?contract=<id>`
- Uses `replaceState` to avoid cluttering browser history

#### ContractPicker Component
- Location: `frontend/src/components/ContractPicker.tsx`
- Searchable dropdown displayed in the application header
- Shows "All contracts" when no contract is selected
- Shows the selected contract name with a clear button (×) when active
- Filters contracts by description or ID as you type

#### Affected Pages
All the following pages now respect the selected contract filter:

1. **Contract Rates** (`/contract-rates`)
   - Filters rates by contract
   - Defaults new rates to the selected contract

2. **Fixed Costs** (`/fixed-costs`)
   - Filters costs by contract
   - Defaults new costs to the selected contract

3. **Tasks** (`/tasks`)
   - Filters tasks by contract
   - Defaults new tasks to the selected contract

4. **Work Records** (`/work-records`)
   - Filters work records by tasks associated with the selected contract
   - Indirect filtering through Task relationship

5. **Invoice Items** (`/invoice-items`)
   - Filters items by tasks or fixed costs associated with the selected contract
   - Indirect filtering through Task/FixedCost relationships

6. **Invoices** (`/invoices`)
   - Filters invoices by the client associated with the selected contract
   - Indirect filtering through Contract's client

### Backend Implementation

All affected API endpoints now accept an optional `contractId` query parameter:

- `GET /api/contractrates?contractId=<id>`
- `GET /api/fixedcosts?contractId=<id>`
- `GET /api/tasks?contractId=<id>`
- `GET /api/workrecords?contractId=<id>` (filters via Task.ContractId)
- `GET /api/invoiceitems?contractId=<id>` (filters via Task or FixedCost)
- `GET /api/invoices?contractId=<id>` (filters via Contract.ClientId)

When the `contractId` parameter is provided, the API returns only items associated with that contract.

## Usage

### Selecting a Contract
1. Click on the contract picker in the header
2. Type to search for a contract by description or ID
3. Click on a contract to select it
4. The URL will update with `?contract=<id>`
5. All list pages will automatically filter to show only items for that contract

### Creating New Items
When a contract is selected:
- New items will have their contract field pre-filled
- This applies to Contract Rates, Fixed Costs, and Tasks

### Clearing the Selection
- Click the (×) button next to the selected contract name in the header
- The `contract` query parameter will be removed from the URL
- All lists will return to showing all items

### Bookmarking/Sharing
The selected contract is represented in the URL, so users can:
- Bookmark a filtered view
- Share a URL that opens the app with a specific contract selected
- Navigate between pages while maintaining the filter

## Error Handling
- If a contract ID in the URL doesn't exist or the user doesn't have access, an error message is displayed
- The invalid contract selection is cleared from the URL
- Lists return to showing all items

## Technical Details

### URL Parameter
- Parameter name: `contract`
- Format: `?contract=<id>` where `<id>` is the numeric contract ID
- Uses `replaceState` to avoid cluttering browser history when toggling the filter

### State Management
- Global state managed via React Context (`ContractSelectionContext`)
- State syncs with URL query parameters
- No localStorage or server-side persistence

### Filter Propagation
- Direct filtering: Contract Rates, Fixed Costs, Tasks (have ContractId field)
- Indirect filtering:
  - Work Records: filtered via Task.ContractId
  - Invoice Items: filtered via Task.ContractId or FixedCost.ContractId
  - Invoices: filtered via Contract.ClientId matching Invoice.ClientId

## Future Enhancements
Potential improvements not included in this implementation:
- Persist selection across sessions via localStorage
- Support for archived/disabled contracts
- Contract-specific permissions
- Creating new contracts from the picker
- Multiple contract selection
- Caching contract-to-client mappings for invoice filtering performance
- Upper bound validation for contract IDs (beyond Number.MAX_SAFE_INTEGER)
