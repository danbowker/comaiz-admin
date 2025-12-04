import { RecordState, Contract, Task } from '../types';

/**
 * LocalStorage keys for filter preferences
 */
export const STORAGE_KEYS = {
  SHOW_COMPLETE_CONTRACTS: 'comaiz.showCompleteContracts',
  SHOW_COMPLETE_TASKS: 'comaiz.showCompleteTasks',
};

/**
 * Check if a contract is active (not complete)
 */
export const isContractActive = (contract: Contract): boolean => {
  return contract.state === RecordState.Active;
};

/**
 * Check if a task is active (not complete)
 */
export const isTaskActive = (task: Task): boolean => {
  return task.state === RecordState.Active;
};

/**
 * Calculate effective active state for a task.
 * A task is effectively active only if both the task itself is active
 * AND its parent contract (if any) is also active.
 */
export const isTaskEffectivelyActive = (
  task: Task,
  contracts: Contract[]
): boolean => {
  // Task itself must be active
  if (!isTaskActive(task)) {
    return false;
  }

  // If task has no contract, it's effectively active if task is active
  if (!task.contractId) {
    return true;
  }

  // Find the parent contract
  const contract = contracts.find((c) => c.id === task.contractId);

  // If contract not found, assume task is active
  if (!contract) {
    return true;
  }

  // Task is effectively active only if contract is also active
  return isContractActive(contract);
};

/**
 * Filter contracts by state
 */
export const filterContractsByState = (
  contracts: Contract[],
  showComplete: boolean
): Contract[] => {
  if (showComplete) {
    return contracts;
  }
  return contracts.filter(isContractActive);
};

/**
 * Filter tasks by effective active state
 */
export const filterTasksByEffectiveState = (
  tasks: Task[],
  contracts: Contract[],
  showComplete: boolean
): Task[] => {
  if (showComplete) {
    return tasks;
  }
  return tasks.filter((task) => isTaskEffectivelyActive(task, contracts));
};

/**
 * Get preference from localStorage
 */
export const getStoredPreference = (key: string, defaultValue: boolean = false): boolean => {
  try {
    const stored = localStorage.getItem(key);
    if (stored === null) {
      return defaultValue;
    }
    return stored === 'true';
  } catch {
    return defaultValue;
  }
};

/**
 * Set preference in localStorage
 */
export const setStoredPreference = (key: string, value: boolean): void => {
  try {
    localStorage.setItem(key, String(value));
  } catch {
    // Ignore localStorage errors
  }
};
