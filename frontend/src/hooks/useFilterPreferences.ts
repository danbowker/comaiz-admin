import { useState, useCallback } from 'react';
import { getStoredPreference, setStoredPreference, STORAGE_KEYS } from '../utils/stateFilter';

/**
 * Custom hook for managing the "show complete contracts" preference
 */
export const useShowCompleteContracts = () => {
  const [showComplete, setShowCompleteState] = useState(() =>
    getStoredPreference(STORAGE_KEYS.SHOW_COMPLETE_CONTRACTS, false)
  );

  const setShowComplete = useCallback((value: boolean) => {
    setShowCompleteState(value);
    setStoredPreference(STORAGE_KEYS.SHOW_COMPLETE_CONTRACTS, value);
  }, []);

  const toggleShowComplete = useCallback(() => {
    setShowComplete(!showComplete);
  }, [showComplete, setShowComplete]);

  return { showComplete, setShowComplete, toggleShowComplete };
};

/**
 * Custom hook for managing the "show complete tasks" preference
 */
export const useShowCompleteTasks = () => {
  const [showComplete, setShowCompleteState] = useState(() =>
    getStoredPreference(STORAGE_KEYS.SHOW_COMPLETE_TASKS, false)
  );

  const setShowComplete = useCallback((value: boolean) => {
    setShowCompleteState(value);
    setStoredPreference(STORAGE_KEYS.SHOW_COMPLETE_TASKS, value);
  }, []);

  const toggleShowComplete = useCallback(() => {
    setShowComplete(!showComplete);
  }, [showComplete, setShowComplete]);

  return { showComplete, setShowComplete, toggleShowComplete };
};
