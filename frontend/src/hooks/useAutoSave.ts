import { useState, useEffect, useRef } from "react";

export type SaveStatus = "saved" | "saving" | "unsaved" | "error";

export function useAutoSave<T>(
  value: T,
  onSave: (val: T) => Promise<void>,
  delay: number = 800
) {
  const [status, setStatus] = useState<SaveStatus>("saved");
  const isFirstRender = useRef(true);
  const timerRef = useRef<any>(null);
  const latestValueRef = useRef(value);

  latestValueRef.current = value;

  useEffect(() => {
    if (isFirstRender.current) {
      isFirstRender.current = false;
      return;
    }

    setStatus("unsaved");
    if (timerRef.current) {
      clearTimeout(timerRef.current);
    }

    timerRef.current = setTimeout(async () => {
      setStatus("saving");
      try {
        await onSave(latestValueRef.current);
        setStatus("saved");
      } catch {
        setStatus("error");
      }
    }, delay);

    return () => {
      if (timerRef.current) {
        clearTimeout(timerRef.current);
      }
    };
  }, [value, delay, onSave]);

  return { status, setStatus };
}
