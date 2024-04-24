toc.dat                                                                                             0000600 0004000 0002000 00000007044 14612204472 0014446 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        PGDMP       ;                |            Category    16.1    16.1     �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false         �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false         �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false         �           1262    34366    Category    DATABASE     }   CREATE DATABASE "Category" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_India.1252';
    DROP DATABASE "Category";
                postgres    false         �            1259    34367    category    TABLE     Z   CREATE TABLE public.category (
    "Id" integer NOT NULL,
    "Name" character varying
);
    DROP TABLE public.category;
       public         heap    postgres    false         �            1259    34374    category_Id_seq    SEQUENCE     �   ALTER TABLE public.category ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."category_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    215         �            1259    34375    taskman    TABLE     �   CREATE TABLE public.taskman (
    taskid integer NOT NULL,
    taskname character varying,
    assignee character varying,
    categoryid integer,
    description character varying,
    duedate timestamp without time zone,
    city character varying
);
    DROP TABLE public.taskman;
       public         heap    postgres    false         �            1259    34382    taskman_taskid_seq    SEQUENCE     �   ALTER TABLE public.taskman ALTER COLUMN taskid ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.taskman_taskid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    217         �          0    34367    category 
   TABLE DATA           0   COPY public.category ("Id", "Name") FROM stdin;
    public          postgres    false    215       4840.dat �          0    34375    taskman 
   TABLE DATA           e   COPY public.taskman (taskid, taskname, assignee, categoryid, description, duedate, city) FROM stdin;
    public          postgres    false    217       4842.dat �           0    0    category_Id_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('public."category_Id_seq"', 5, true);
          public          postgres    false    216         �           0    0    taskman_taskid_seq    SEQUENCE SET     @   SELECT pg_catalog.setval('public.taskman_taskid_seq', 9, true);
          public          postgres    false    218         V           2606    34371    category category_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.category
    ADD CONSTRAINT category_pkey PRIMARY KEY ("Id");
 @   ALTER TABLE ONLY public.category DROP CONSTRAINT category_pkey;
       public            postgres    false    215         X           2606    34381    taskman task_pkey 
   CONSTRAINT     S   ALTER TABLE ONLY public.taskman
    ADD CONSTRAINT task_pkey PRIMARY KEY (taskid);
 ;   ALTER TABLE ONLY public.taskman DROP CONSTRAINT task_pkey;
       public            postgres    false    217                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    4840.dat                                                                                            0000600 0004000 0002000 00000000047 14612204472 0014254 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        1	e3r
2	wqfwq
3	e3rr
4	ww
5	rehre
\.


                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         4842.dat                                                                                            0000600 0004000 0002000 00000000266 14612204472 0014261 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        4	ewfe	fwef	1	ewfew	0001-01-11 00:00:00	Delhi
5	wewe	wfwq	2	wqf	0001-01-10 00:00:00	Mumbai
3	g4g4	4g43	3	4g34	0001-01-04 00:00:00	Mumbai
6	ey	y	1	ery	0001-01-03 00:00:00	Mumbai
\.


                                                                                                                                                                                                                                                                                                                                          restore.sql                                                                                         0000600 0004000 0002000 00000007126 14612204472 0015374 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        --
-- NOTE:
--
-- File paths need to be edited. Search for $$PATH$$ and
-- replace it with the path to the directory containing
-- the extracted data files.
--
--
-- PostgreSQL database dump
--

-- Dumped from database version 16.1
-- Dumped by pg_dump version 16.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP DATABASE "Category";
--
-- Name: Category; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE "Category" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_India.1252';


ALTER DATABASE "Category" OWNER TO postgres;

\connect "Category"

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: category; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.category (
    "Id" integer NOT NULL,
    "Name" character varying
);


ALTER TABLE public.category OWNER TO postgres;

--
-- Name: category_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.category ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."category_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: taskman; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.taskman (
    taskid integer NOT NULL,
    taskname character varying,
    assignee character varying,
    categoryid integer,
    description character varying,
    duedate timestamp without time zone,
    city character varying
);


ALTER TABLE public.taskman OWNER TO postgres;

--
-- Name: taskman_taskid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.taskman ALTER COLUMN taskid ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.taskman_taskid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Data for Name: category; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.category ("Id", "Name") FROM stdin;
\.
COPY public.category ("Id", "Name") FROM '$$PATH$$/4840.dat';

--
-- Data for Name: taskman; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.taskman (taskid, taskname, assignee, categoryid, description, duedate, city) FROM stdin;
\.
COPY public.taskman (taskid, taskname, assignee, categoryid, description, duedate, city) FROM '$$PATH$$/4842.dat';

--
-- Name: category_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."category_Id_seq"', 5, true);


--
-- Name: taskman_taskid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.taskman_taskid_seq', 9, true);


--
-- Name: category category_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.category
    ADD CONSTRAINT category_pkey PRIMARY KEY ("Id");


--
-- Name: taskman task_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.taskman
    ADD CONSTRAINT task_pkey PRIMARY KEY (taskid);


--
-- PostgreSQL database dump complete
--

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          