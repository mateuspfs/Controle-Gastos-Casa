import { useState, useRef, useEffect, type ChangeEvent, type KeyboardEvent } from 'react';
import { createPortal } from 'react-dom';
import { ChevronDownIcon, MagnifyingGlassIcon } from '@heroicons/react/24/outline';

interface SelectOption {
  value: string | number;
  label: string;
}

interface SelectProps {
  label?: string;
  error?: string;
  options: SelectOption[];
  placeholder?: string;
  value?: string | number;
  onChange?: (e: ChangeEvent<HTMLInputElement>) => void;
  onBlur?: () => void;
  disabled?: boolean;
  required?: boolean;
  className?: string;
  name?: string;
  id?: string;
}

// Componente de select com busca/filtro enquanto digita
const Select = ({
  label,
  error,
  options,
  placeholder,
  value,
  onChange,
  onBlur,
  disabled = false,
  required = false,
  className = '',
  name,
  id,
}: SelectProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedOption, setSelectedOption] = useState<SelectOption | null>(null);
  const [dropdownPosition, setDropdownPosition] = useState({ top: 0, left: 0, width: 0 });
  const containerRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const option = options.find((opt) => {
      if (typeof value === 'number' && typeof opt.value === 'number') {
        return opt.value === value;
      }
      if (typeof value === 'string' && typeof opt.value === 'string') {
        return opt.value === value;
      }
      return String(opt.value) === String(value);
    });

    if (option) {
      setSelectedOption(option);
      setSearchTerm(option.label);
    } else {
      setSelectedOption(null);
      setSearchTerm(placeholder ? '' : '');
    }
  }, [value, options, placeholder]);

  const filteredOptions = searchTerm.trim()
    ? options.filter((option) =>
        option.label.toLowerCase().includes(searchTerm.toLowerCase())
      )
    : options;

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      const target = event.target as Node;
      const clickedInContainer = containerRef.current?.contains(target);
      const clickedInDropdown = dropdownRef.current?.contains(target);
      
      if (!clickedInContainer && !clickedInDropdown) {
        setIsOpen(false);
        if (selectedOption) {
          setSearchTerm(selectedOption.label);
        } else {
          setSearchTerm('');
        }
      }
    };

    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside);
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [isOpen, selectedOption]);

  const handleSelect = (option: SelectOption, e?: React.MouseEvent) => {
    if (e) {
      e.preventDefault();
      e.stopPropagation();
    }
    
    setSelectedOption(option);
    if (option.value === '' || option.value === null || option.value === undefined) {
      setSearchTerm('');
    } else {
      setSearchTerm(option.label);
    }
    setIsOpen(false);

    if (inputRef.current) {
      inputRef.current.blur();
    }

    if (onChange) {
      const syntheticEvent = {
        target: {
          value: option.value === '' ? '' : option.value.toString(),
          name: name || '',
        },
      } as ChangeEvent<HTMLInputElement>;
      onChange(syntheticEvent);
    }
  };

  const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    setSearchTerm(newValue);
    setIsOpen(true);

    if (newValue === '') {
      setSelectedOption(null);
      if (onChange) {
        const syntheticEvent = {
          target: {
            value: '',
            name: name || '',
          },
        } as ChangeEvent<HTMLInputElement>;
        onChange(syntheticEvent);
      }
    }
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter' && filteredOptions.length > 0) {
      e.preventDefault();
      handleSelect(filteredOptions[0]);
    } else if (e.key === 'Escape') {
      setIsOpen(false);
      if (selectedOption) {
        setSearchTerm(selectedOption.label);
      }
    } else if (e.key === 'ArrowDown') {
      e.preventDefault();
      setIsOpen(true);
    }
  };

  useEffect(() => {
    if (isOpen && inputRef.current) {
      inputRef.current.focus();
      updateDropdownPosition();
    }
  }, [isOpen]);

  const updateDropdownPosition = () => {
    if (inputRef.current) {
      const rect = inputRef.current.getBoundingClientRect();
      setDropdownPosition({
        top: rect.bottom + window.scrollY + 4,
        left: rect.left + window.scrollX,
        width: rect.width,
      });
    }
  };

  useEffect(() => {
    if (isOpen) {
      updateDropdownPosition();
      const handleScroll = () => updateDropdownPosition();
      const handleResize = () => updateDropdownPosition();
      window.addEventListener('scroll', handleScroll, true);
      window.addEventListener('resize', handleResize);
      return () => {
        window.removeEventListener('scroll', handleScroll, true);
        window.removeEventListener('resize', handleResize);
      };
    }
  }, [isOpen]);

  return (
    <div className="w-full" ref={containerRef}>
      {label && (
        <label
          htmlFor={id}
          className="mb-1.5 block text-sm font-medium text-slate-700 dark:text-slate-300"
        >
          {label}
          {required && <span className="ml-1 text-red-500">*</span>}
        </label>
      )}
      <div className="relative">
        <div className="relative">
          <div className="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
            <MagnifyingGlassIcon className="h-5 w-5 text-slate-400" />
          </div>
          <input
            ref={inputRef}
            type="text"
            id={id}
            name={name}
            value={searchTerm}
            onChange={handleInputChange}
            onFocus={(e) => {
              setIsOpen(true);
              e.target.select();
              setSearchTerm('');
            }}
            onBlur={() => {
              setTimeout(() => {
                if (onBlur) onBlur();
              }, 200);
            }}
            onKeyDown={handleKeyDown}
            disabled={disabled}
            placeholder={placeholder || 'Digite para buscar...'}
            className={`w-full rounded-lg border pl-10 pr-10 py-2.5 text-sm transition-colors focus:outline-none focus:ring-2 focus:ring-offset-1 ${
              error
                ? 'border-red-300 bg-red-50 text-red-900 placeholder-red-300 focus:border-red-500 focus:ring-red-500 dark:border-red-700 dark:bg-red-900/20 dark:text-red-200 dark:placeholder-red-500'
                : 'border-slate-300 bg-white text-slate-900 placeholder-slate-400 focus:border-brand-500 focus:ring-brand-500 dark:border-slate-600 dark:bg-slate-800 dark:text-slate-100 dark:placeholder-slate-500'
            } ${disabled ? 'cursor-not-allowed opacity-50' : ''} ${className}`}
          />
          <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
            <ChevronDownIcon
              className={`h-5 w-5 text-slate-400 transition-transform ${
                isOpen ? 'transform rotate-180' : ''
              }`}
            />
          </div>
        </div>

        {isOpen && !disabled && createPortal(
          <div
            ref={dropdownRef}
            className="fixed z-[9999] bg-white border border-slate-300 rounded-lg shadow-lg max-h-60 overflow-auto dark:bg-slate-800 dark:border-slate-600"
            style={{
              top: `${dropdownPosition.top}px`,
              left: `${dropdownPosition.left}px`,
              width: `${dropdownPosition.width}px`,
            }}
          >
            {filteredOptions.length > 0 ? (
              <ul className="py-1 text-sm text-slate-900 dark:text-slate-100">
                {filteredOptions.map((option) => (
                  <li
                    key={option.value}
                    onMouseDown={(e) => {
                      e.preventDefault();
                      handleSelect(option, e);
                    }}
                    className={`px-4 py-2 cursor-pointer hover:bg-slate-100 dark:hover:bg-slate-700 ${
                      selectedOption?.value === option.value
                        ? 'bg-brand-50 text-brand-800 dark:bg-brand-900/40 dark:text-brand-50'
                        : ''
                    }`}
                  >
                    {option.label}
                  </li>
                ))}
              </ul>
            ) : (
              <div className="px-4 py-2 text-sm text-slate-500 dark:text-slate-400">
                Nenhuma opção encontrada
              </div>
            )}
          </div>,
          document.body
        )}
      </div>
      {error && (
        <p className="mt-1.5 text-xs font-medium text-red-600 dark:text-red-400">
          {error}
        </p>
      )}
    </div>
  );
};

export default Select;

