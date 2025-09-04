import { useState, useEffect } from 'react';
import { apiService } from '../services/apiService';

export function useApi<T>(
  apiCall: () => Promise<T[]>,
  deps: any[] = []
) {
  const [data, setData] = useState<T[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);
        const result = await apiCall();
        if (isMounted) {
          setData(result);
        }
      } catch (err) {
        if (isMounted) {
          setError(err instanceof Error ? err.message : 'An error occurred');
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    fetchData();

    return () => {
      isMounted = false;
    };
  }, deps);

  const refetch = () => {
    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);
        const result = await apiCall();
        setData(result);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  };

  return { data, loading, error, refetch };
}

export function useCrud<T extends { id: number }>(
  getAll: () => Promise<T[]>,
  create: (item: Omit<T, 'id'>) => Promise<T>,
  update: (item: T) => Promise<void>,
  deleteItem: (id: number) => Promise<void>
) {
  const { data, loading, error, refetch } = useApi(getAll);
  const [submitting, setSubmitting] = useState(false);

  const handleCreate = async (item: Omit<T, 'id'>) => {
    try {
      setSubmitting(true);
      await create(item);
      await refetch();
    } catch (err) {
      throw err;
    } finally {
      setSubmitting(false);
    }
  };

  const handleUpdate = async (item: T) => {
    try {
      setSubmitting(true);
      await update(item);
      await refetch();
    } catch (err) {
      throw err;
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setSubmitting(true);
      await deleteItem(id);
      await refetch();
    } catch (err) {
      throw err;
    } finally {
      setSubmitting(false);
    }
  };

  return {
    data,
    loading,
    error,
    submitting,
    refetch,
    create: handleCreate,
    update: handleUpdate,
    delete: handleDelete,
  };
}