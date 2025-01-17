
import React from 'react';
import { Table as MuiTable, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper } from '@mui/material';
import { TableStyle } from './TableStyle';


interface Column {
  label: string;
  accessor?: string; 
  render?: (row: any) => React.ReactNode; 
}

interface TableProps {
  columns: Column[]; 
  data: any[]; 
}

const Table: React.FC<TableProps> = ({ columns, data }) => {

  // console.log(data)
  return (
    <TableContainer
      component={Paper} 
      sx={{
        maxHeight: 600, 
      }}
    >
      <MuiTable stickyHeader sx={{ minWidth: 600 }}>
        {/* Table Header */}
        <TableHead>
          <TableRow>
            {columns.map((column, index) => (
              <TableCell
                key={index}
                sx={TableStyle.tableRow}
              >
                {column.label}
              </TableCell>
            ))}
          </TableRow>
        </TableHead>

        {/* Table Body */}
        <TableBody>
          {data.length > 0 ? (
            data.map((row, rowIndex) => (
              <TableRow key={rowIndex}
              sx={{
                ...(row.isHighlight === true && { backgroundColor: 'lightblue' }), // Apply highlight only if IsHighlight is explicitly true
              }}
              >
                {columns.map((column, colIndex) => (
                  <TableCell key={colIndex} sx={TableStyle.tableCell}>
                    {column.render ? column.render(row) : row[column.accessor || '']}
                  </TableCell>
                ))}
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={columns.length} align="center">
                No data available
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </MuiTable>
    </TableContainer>
  );
};

export default Table;
