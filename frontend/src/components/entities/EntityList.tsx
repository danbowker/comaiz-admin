import React, { useState, useEffect, useCallback } from 'react';
import { EntityService } from '../../services/entityService';
import './EntityList.css';

interface Column<T> {
  key: keyof T | string;
  label: string;
  render?: (item: T) => React.ReactNode;
}

interface FilterToggleProps {
  label: string;
  checked: boolean;
  onChange: () => void;
}

interface EntityListProps<T extends { id: number }> {
  title: string;
  service: EntityService<T>;
  columns: Column<T>[];
  onEdit: (item: T) => void;
  onCreate: () => void;
  onDelete: (id: number) => void;
  onDuplicate?: (id: number) => void;
  queryParams?: Record<string, any>;
  getRowClassName?: (item: T) => string;
  filterToggle?: FilterToggleProps;
}

function EntityList<T extends { id: number }>({
  title,
  service,
  columns,
  onEdit,
  onCreate,
  onDelete,
  onDuplicate,
  queryParams,
  getRowClassName,
  filterToggle,
}: EntityListProps<T>) {
  const [items, setItems] = useState<T[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const loadItems = useCallback(async () => {
    try {
      setLoading(true);
      const data = await service.getAll(queryParams);
      setItems(data);
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load items');
    } finally {
      setLoading(false);
    }
  }, [service, queryParams]);

  useEffect(() => {
    loadItems();
  }, [loadItems]);

  const handleDelete = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this item?')) {
      try {
        await onDelete(id);
        await loadItems();
      } catch (err: any) {
        setError(err.response?.data?.message || 'Failed to delete item');
      }
    }
  };

  const handleDuplicate = async (id: number) => {
    if (onDuplicate) {
      try {
        await onDuplicate(id);
        await loadItems();
      } catch (err: any) {
        setError(err.response?.data?.message || 'Failed to duplicate item');
      }
    }
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div className="entity-list">
      <div className="entity-header">
        <h2>{title}</h2>
        <div className="entity-header-controls">
          {filterToggle && (
            <label className="filter-toggle">
              <input
                type="checkbox"
                checked={filterToggle.checked}
                onChange={filterToggle.onChange}
                aria-label={filterToggle.label}
              />
              <span>{filterToggle.label}</span>
            </label>
          )}
          <button className="btn-primary" onClick={onCreate}>
            Create New
          </button>
        </div>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="table-container">
        <table>
          <thead>
            <tr>
              {columns.map((col) => (
                <th key={String(col.key)}>{col.label}</th>
              ))}
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {items.length === 0 ? (
              <tr>
                <td colSpan={columns.length + 1} className="no-data">
                  No items found
                </td>
              </tr>
            ) : (
              items.map((item) => (
                <tr key={item.id} className={getRowClassName ? getRowClassName(item) : ''}>
                  {columns.map((col) => (
                    <td key={String(col.key)}>
                      {col.render
                        ? col.render(item)
                        : String((item as any)[col.key] ?? '')}
                    </td>
                  ))}
                  <td className="actions">
                    <button
                      className="btn-edit"
                      onClick={() => onEdit(item)}
                    >
                      Edit
                    </button>
                    {onDuplicate && (
                      <button
                        className="btn-duplicate"
                        onClick={() => handleDuplicate(item.id)}
                      >
                        Duplicate
                      </button>
                    )}
                    <button
                      className="btn-delete"
                      onClick={() => handleDelete(item.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default EntityList;
