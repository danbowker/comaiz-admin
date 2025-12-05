import {
  isContractActive,
  isTaskActive,
  isTaskEffectivelyActive,
  filterContractsByState,
  filterTasksByEffectiveState,
  getStoredPreference,
  setStoredPreference,
  STORAGE_KEYS,
} from '../utils/stateFilter';
import { Contract, Task, RecordState, ChargeType } from '../types';

// Mock localStorage
const localStorageMock = (() => {
  let store: Record<string, string> = {};
  return {
    getItem: (key: string) => store[key] || null,
    setItem: (key: string, value: string) => {
      store[key] = value;
    },
    removeItem: (key: string) => {
      delete store[key];
    },
    clear: () => {
      store = {};
    },
  };
})();

Object.defineProperty(window, 'localStorage', { value: localStorageMock });

describe('stateFilter utility functions', () => {
  const createContract = (id: number, state: RecordState): Contract => ({
    id,
    clientId: 1,
    description: `Contract ${id}`,
    chargeType: ChargeType.Fixed,
    state,
  });

  const createTask = (id: number, state: RecordState, contractId?: number): Task => ({
    id,
    name: `Task ${id}`,
    state,
    contractId,
  });

  describe('isContractActive', () => {
    it('should return true for active contracts', () => {
      const contract = createContract(1, RecordState.Active);
      expect(isContractActive(contract)).toBe(true);
    });

    it('should return false for complete contracts', () => {
      const contract = createContract(1, RecordState.Complete);
      expect(isContractActive(contract)).toBe(false);
    });
  });

  describe('isTaskActive', () => {
    it('should return true for active tasks', () => {
      const task = createTask(1, RecordState.Active);
      expect(isTaskActive(task)).toBe(true);
    });

    it('should return false for complete tasks', () => {
      const task = createTask(1, RecordState.Complete);
      expect(isTaskActive(task)).toBe(false);
    });
  });

  describe('isTaskEffectivelyActive', () => {
    it('should return true for active task with no contract', () => {
      const task = createTask(1, RecordState.Active);
      expect(isTaskEffectivelyActive(task, [])).toBe(true);
    });

    it('should return true for active task with active contract', () => {
      const contract = createContract(1, RecordState.Active);
      const task = createTask(1, RecordState.Active, 1);
      expect(isTaskEffectivelyActive(task, [contract])).toBe(true);
    });

    it('should return false for active task with complete contract', () => {
      const contract = createContract(1, RecordState.Complete);
      const task = createTask(1, RecordState.Active, 1);
      expect(isTaskEffectivelyActive(task, [contract])).toBe(false);
    });

    it('should return false for complete task regardless of contract state', () => {
      const contract = createContract(1, RecordState.Active);
      const task = createTask(1, RecordState.Complete, 1);
      expect(isTaskEffectivelyActive(task, [contract])).toBe(false);
    });

    it('should return true for active task when contract not found', () => {
      const task = createTask(1, RecordState.Active, 999);
      expect(isTaskEffectivelyActive(task, [])).toBe(true);
    });
  });

  describe('filterContractsByState', () => {
    const contracts = [
      createContract(1, RecordState.Active),
      createContract(2, RecordState.Complete),
      createContract(3, RecordState.Active),
    ];

    it('should return only active contracts when showComplete is false', () => {
      const result = filterContractsByState(contracts, false);
      expect(result).toHaveLength(2);
      expect(result.map(c => c.id)).toEqual([1, 3]);
    });

    it('should return all contracts when showComplete is true', () => {
      const result = filterContractsByState(contracts, true);
      expect(result).toHaveLength(3);
    });
  });

  describe('filterTasksByEffectiveState', () => {
    const contracts = [
      createContract(1, RecordState.Active),
      createContract(2, RecordState.Complete),
    ];

    const tasks = [
      createTask(1, RecordState.Active, 1),    // Active task, active contract
      createTask(2, RecordState.Active, 2),    // Active task, complete contract
      createTask(3, RecordState.Complete, 1),  // Complete task, active contract
      createTask(4, RecordState.Active),       // Active task, no contract
    ];

    it('should return only effectively active tasks when showComplete is false', () => {
      const result = filterTasksByEffectiveState(tasks, contracts, false);
      expect(result).toHaveLength(2);
      expect(result.map(t => t.id)).toEqual([1, 4]);
    });

    it('should return all tasks when showComplete is true', () => {
      const result = filterTasksByEffectiveState(tasks, contracts, true);
      expect(result).toHaveLength(4);
    });
  });

  describe('localStorage preferences', () => {
    beforeEach(() => {
      localStorageMock.clear();
    });

    describe('getStoredPreference', () => {
      it('should return default value when key not in localStorage', () => {
        expect(getStoredPreference('nonexistent', false)).toBe(false);
        expect(getStoredPreference('nonexistent', true)).toBe(true);
      });

      it('should return true when localStorage has "true"', () => {
        localStorageMock.setItem(STORAGE_KEYS.SHOW_COMPLETE_CONTRACTS, 'true');
        expect(getStoredPreference(STORAGE_KEYS.SHOW_COMPLETE_CONTRACTS)).toBe(true);
      });

      it('should return false when localStorage has "false"', () => {
        localStorageMock.setItem(STORAGE_KEYS.SHOW_COMPLETE_CONTRACTS, 'false');
        expect(getStoredPreference(STORAGE_KEYS.SHOW_COMPLETE_CONTRACTS)).toBe(false);
      });
    });

    describe('setStoredPreference', () => {
      it('should store true as "true"', () => {
        setStoredPreference(STORAGE_KEYS.SHOW_COMPLETE_CONTRACTS, true);
        expect(localStorageMock.getItem(STORAGE_KEYS.SHOW_COMPLETE_CONTRACTS)).toBe('true');
      });

      it('should store false as "false"', () => {
        setStoredPreference(STORAGE_KEYS.SHOW_COMPLETE_CONTRACTS, false);
        expect(localStorageMock.getItem(STORAGE_KEYS.SHOW_COMPLETE_CONTRACTS)).toBe('false');
      });
    });
  });

  describe('STORAGE_KEYS', () => {
    it('should have correct keys', () => {
      expect(STORAGE_KEYS.SHOW_COMPLETE_CONTRACTS).toBe('comaiz.showCompleteContracts');
      expect(STORAGE_KEYS.SHOW_COMPLETE_TASKS).toBe('comaiz.showCompleteTasks');
    });
  });
});
